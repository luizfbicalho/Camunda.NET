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
//	import static org.hamcrest.CoreMatchers.either;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
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

	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using MockHistoricVariableInstanceBuilder = org.camunda.bpm.engine.rest.helper.MockHistoricVariableInstanceBuilder;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
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
	public class HistoricVariableInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/variable-instance";
	  protected internal static readonly string VARIABLE_INSTANCE_URL = HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL + "/{id}";
	  protected internal static readonly string VARIABLE_INSTANCE_BINARY_DATA_URL = VARIABLE_INSTANCE_URL + "/data";

	  protected internal HistoryService historyServiceMock;

	  protected internal HistoricVariableInstanceQuery variableInstanceQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupTestData()
	  public virtual void setupTestData()
	  {
		historyServiceMock = mock(typeof(HistoryService));
		variableInstanceQueryMock = mock(typeof(HistoricVariableInstanceQuery));

		// mock engine service.
		when(processEngine.HistoryService).thenReturn(historyServiceMock);
		when(historyServiceMock.createHistoricVariableInstanceQuery()).thenReturn(variableInstanceQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstance()
	  public virtual void testGetSingleVariableInstance()
	  {
		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance();

		HistoricVariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", builder.Id).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo(builder.Value)).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("executionId", equalTo(builder.ExecutionId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("tenantId", equalTo(builder.TenantId)).body("createTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME)).body("removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME)).body("rootProcessInstanceId", equalTo(builder.RootProcessInstanceId)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceDeserialized()
	  public virtual void testGetSingleVariableInstanceDeserialized()
	  {
		ObjectValue serializedValue = MockObjectValue.fromObjectValue(Variables.objectValue("a value").serializationDataFormat("aDataFormat").create()).objectTypeName("aTypeName");

		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance().typedValue(serializedValue);
		HistoricVariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("executionId", equalTo(builder.ExecutionId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("tenantId", equalTo(builder.TenantId)).body("createTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME)).body("removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME)).body("rootProcessInstanceId", equalTo(builder.RootProcessInstanceId)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
		verify(variableInstanceQueryMock, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceSerialized()
	  public virtual void testGetSingleVariableInstanceSerialized()
	  {
		ObjectValue serializedValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("aDataFormat").objectTypeName("aTypeName").create();

		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance().typedValue(serializedValue);
		HistoricVariableInstance variableInstanceMock = builder.build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("name", equalTo(builder.Name)).body("type", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a serialized value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("executionId", equalTo(builder.ExecutionId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("taskId", equalTo(builder.TaskId)).body("tenantId", equalTo(builder.TenantId)).body("createTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME)).body("removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME)).body("rootProcessInstanceId", equalTo(builder.RootProcessInstanceId)).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
		verify(variableInstanceQueryMock, times(1)).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceForBinaryVariable()
	  public virtual void testGetSingleVariableInstanceForBinaryVariable()
	  {
		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance();

		HistoricVariableInstance variableInstanceMock = builder.typedValue(Variables.byteArrayValue(null)).build();

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

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Historic variable instance with Id 'nonExistingId' does not exist.")).when().get(VARIABLE_INSTANCE_URL);

		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForBinaryVariable()
	  public virtual void testBinaryDataForBinaryVariable()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();
		HistoricVariableInstance variableInstanceMock = MockProvider.mockHistoricVariableInstance().typedValue(Variables.byteArrayValue(byteContent)).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.BINARY.ToString()).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		sbyte[] responseBytes = response.Body.asByteArray();
		Assert.assertEquals(StringHelper.NewString(byteContent), StringHelper.NewString(responseBytes));
		verify(variableInstanceQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForFileVariable()
	  public virtual void testGetBinaryDataForFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = "test".GetBytes();
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();
		HistoricVariableInstance variableInstanceMock = MockProvider.mockHistoricVariableInstance().typedValue(variableValue).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body(@is(equalTo(StringHelper.NewString(byteContent)))).header("Content-Disposition", "attachment; filename=" + filename).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);
		//due to some problems with wildfly we gotta check this separately
		string contentType = response.ContentType;
		assertThat(contentType, @is(either(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + "; charset=UTF-8")).or(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + ";charset=UTF-8"))));

		verify(variableInstanceQueryMock, never()).disableBinaryFetching();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForNonBinaryVariable()
	  public virtual void testBinaryDataForNonBinaryVariable()
	  {
		HistoricVariableInstance variableInstanceMock = MockProvider.createMockHistoricVariableInstance();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Value of variable with id " + variableInstanceMock.Id + " is not a binary value")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(variableInstanceQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNonExistingVariableInstance()
	  public virtual void testGetBinaryDataForNonExistingVariableInstance()
	  {

		string nonExistingId = "nonExistingId";

		when(variableInstanceQueryMock.variableId(nonExistingId)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Historic variable instance with Id 'nonExistingId' does not exist.")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(variableInstanceQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNullFileVariable()
	  public virtual void testGetBinaryDataForNullFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = null;
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).create();

		HistoricVariableInstance variableInstanceMock = MockProvider.mockHistoricVariableInstance().typedValue(variableValue).build();

		when(variableInstanceQueryMock.variableId(variableInstanceMock.Id)).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).and().contentType(ContentType.TEXT).and().body(@is(equalTo(""))).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteSingleVariableInstanceById()
	  public virtual void testDeleteSingleVariableInstanceById()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID).expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(VARIABLE_INSTANCE_URL);

		verify(historyServiceMock).deleteHistoricVariableInstance(MockProvider.EXAMPLE_VARIABLE_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingVariableInstanceById()
	  public virtual void testDeleteNonExistingVariableInstanceById()
	  {
		doThrow(new NotFoundException("No historic variable instance found with id: 'NON_EXISTING_ID'")).when(historyServiceMock).deleteHistoricVariableInstance("NON_EXISTING_ID");

		given().pathParam("id", "NON_EXISTING_ID").expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("No historic variable instance found with id: 'NON_EXISTING_ID'")).when().delete(VARIABLE_INSTANCE_URL);
	  }
	}

}