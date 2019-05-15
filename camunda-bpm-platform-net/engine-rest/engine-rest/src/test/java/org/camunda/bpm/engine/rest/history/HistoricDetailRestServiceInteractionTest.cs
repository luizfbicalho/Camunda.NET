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
//	import static org.hamcrest.CoreMatchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.either;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
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

	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricVariableUpdate = org.camunda.bpm.engine.history.HistoricVariableUpdate;
	using MockHistoricVariableUpdateBuilder = org.camunda.bpm.engine.rest.helper.MockHistoricVariableUpdateBuilder;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
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
	public class HistoricDetailRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_DETAIL_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/detail";
	  protected internal static readonly string HISTORIC_DETAIL_URL = HISTORIC_DETAIL_RESOURCE_URL + "/{id}";
	  protected internal static readonly string VARIABLE_INSTANCE_BINARY_DATA_URL = HISTORIC_DETAIL_URL + "/data";

	  protected internal HistoryService historyServiceMock;

	  protected internal HistoricDetailQuery historicDetailQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setupTestData()
	  public virtual void setupTestData()
	  {
		historyServiceMock = mock(typeof(HistoryService));
		historicDetailQueryMock = mock(typeof(HistoricDetailQuery));

		// mock engine service.
		when(processEngine.HistoryService).thenReturn(historyServiceMock);
		when(historyServiceMock.createHistoricDetailQuery()).thenReturn(historicDetailQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleDetail()
	  public virtual void testGetSingleDetail()
	  {
		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate();

		HistoricVariableUpdate detailMock = builder.build();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableBinaryFetching()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("variableName", equalTo(builder.Name)).body("variableInstanceId", equalTo(builder.VariableInstanceId)).body("variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo(builder.Value)).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("revision", equalTo(builder.Revision)).body("time", equalTo(builder.Time)).body("taskId", equalTo(builder.TaskId)).body("executionId", equalTo(builder.ExecutionId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("tenantId", equalTo(builder.TenantId)).when().get(HISTORIC_DETAIL_URL);

		verify(historicDetailQueryMock, times(1)).disableBinaryFetching();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableUpdateDeserialized()
	  public virtual void testGetSingleVariableUpdateDeserialized()
	  {
		ObjectValue serializedValue = MockObjectValue.fromObjectValue(Variables.objectValue("a value").serializationDataFormat("aDataFormat").create()).objectTypeName("aTypeName");

		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate().typedValue(serializedValue);
		HistoricVariableUpdate variableInstanceMock = builder.build();

		when(historicDetailQueryMock.detailId(variableInstanceMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableBinaryFetching()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", builder.Id).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("variableName", equalTo(builder.Name)).body("variableInstanceId", equalTo(builder.VariableInstanceId)).body("variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("revision", equalTo(builder.Revision)).body("time", equalTo(builder.Time)).body("taskId", equalTo(builder.TaskId)).body("executionId", equalTo(builder.ExecutionId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("tenantId", equalTo(builder.TenantId)).when().get(HISTORIC_DETAIL_URL);

		verify(historicDetailQueryMock, times(1)).disableBinaryFetching();
		verify(historicDetailQueryMock, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableUpdateSerialized()
	  public virtual void testGetSingleVariableUpdateSerialized()
	  {
		ObjectValue serializedValue = Variables.serializedObjectValue("a serialized value").serializationDataFormat("aDataFormat").objectTypeName("aTypeName").create();

		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate().typedValue(serializedValue);
		HistoricVariableUpdate variableInstanceMock = builder.build();

		when(historicDetailQueryMock.detailId(variableInstanceMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableBinaryFetching()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(variableInstanceMock);

		given().pathParam("id", builder.Id).queryParam("deserializeValue", false).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("variableName", equalTo(builder.Name)).body("variableInstanceId", equalTo(builder.VariableInstanceId)).body("variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo("a serialized value")).body("valueInfo.serializationDataFormat", equalTo("aDataFormat")).body("valueInfo.objectTypeName", equalTo("aTypeName")).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("revision", equalTo(builder.Revision)).body("time", equalTo(builder.Time)).body("taskId", equalTo(builder.TaskId)).body("executionId", equalTo(builder.ExecutionId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("tenantId", equalTo(builder.TenantId)).when().get(HISTORIC_DETAIL_URL);

		verify(historicDetailQueryMock, times(1)).disableBinaryFetching();
		verify(historicDetailQueryMock, times(1)).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleVariableInstanceForBinaryVariable()
	  public virtual void testGetSingleVariableInstanceForBinaryVariable()
	  {
		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate();

		HistoricVariableUpdate detailMock = builder.typedValue(Variables.byteArrayValue(null)).build();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableBinaryFetching()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.OK.StatusCode).and().body("id", equalTo(builder.Id)).body("variableName", equalTo(builder.Name)).body("variableInstanceId", equalTo(builder.VariableInstanceId)).body("variableType", equalTo(VariableTypeHelper.toExpectedValueTypeName(builder.TypedValue.Type))).body("value", equalTo(builder.Value)).body("processDefinitionKey", equalTo(builder.ProcessDefinitionKey)).body("processDefinitionId", equalTo(builder.ProcessDefinitionId)).body("processInstanceId", equalTo(builder.ProcessInstanceId)).body("errorMessage", equalTo(builder.ErrorMessage)).body("activityInstanceId", equalTo(builder.ActivityInstanceId)).body("revision", equalTo(builder.Revision)).body("time", equalTo(builder.Time)).body("taskId", equalTo(builder.TaskId)).body("executionId", equalTo(builder.ExecutionId)).body("caseDefinitionKey", equalTo(builder.CaseDefinitionKey)).body("caseDefinitionId", equalTo(builder.CaseDefinitionId)).body("caseInstanceId", equalTo(builder.CaseInstanceId)).body("caseExecutionId", equalTo(builder.CaseExecutionId)).body("tenantId", equalTo(builder.TenantId)).when().get(HISTORIC_DETAIL_URL);

		verify(historicDetailQueryMock, times(1)).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingVariableInstance()
	  public virtual void testGetNonExistingVariableInstance()
	  {

		string nonExistingId = "nonExistingId";

		when(historicDetailQueryMock.detailId(nonExistingId)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableBinaryFetching()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Historic detail with Id 'nonExistingId' does not exist.")).when().get(HISTORIC_DETAIL_URL);

		verify(historicDetailQueryMock, times(1)).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForBinaryVariable()
	  public virtual void testBinaryDataForBinaryVariable()
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final byte[] byteContent = "some bytes".getBytes();
		sbyte[] byteContent = "some bytes".GetBytes();

		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate();

		HistoricVariableUpdate detailMock = builder.typedValue(Variables.byteArrayValue(byteContent)).build();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.BINARY.ToString()).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		sbyte[] responseBytes = response.Body.asByteArray();
		Assert.assertEquals(StringHelper.NewString(byteContent), StringHelper.NewString(responseBytes));
		verify(historicDetailQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForFileVariable()
	  public virtual void testBinaryDataForFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = "test".GetBytes();
		string encoding = "UTF-8";
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).encoding(encoding).create();

		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate();

		HistoricVariableUpdate detailMock = builder.typedValue(variableValue).build();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.OK.StatusCode).body(@is(equalTo(StringHelper.NewString(byteContent)))).and().header("Content-Disposition", "attachment; filename=" + filename).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);
		//due to some problems with wildfly we gotta check this separately
		string contentType = response.ContentType;
		assertThat(contentType, @is(either(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + "; charset=UTF-8")).or(CoreMatchers.equalTo<object>(ContentType.TEXT.ToString() + ";charset=UTF-8"))));

		verify(historicDetailQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBinaryDataForNonBinaryVariable()
	  public virtual void testBinaryDataForNonBinaryVariable()
	  {
		HistoricVariableUpdate detailMock = MockProvider.createMockHistoricVariableUpdate();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body(containsString("Value of variable with id aHistoricVariableUpdateId is not a binary value")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(historicDetailQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNonExistingVariableInstance()
	  public virtual void testGetBinaryDataForNonExistingVariableInstance()
	  {

		string nonExistingId = "nonExistingId";

		when(historicDetailQueryMock.detailId(nonExistingId)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", nonExistingId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body(containsString("Historic detail with Id '" + nonExistingId + "' does not exist")).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

		verify(historicDetailQueryMock, never()).disableBinaryFetching();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetBinaryDataForNullFileVariable()
	  public virtual void testGetBinaryDataForNullFileVariable()
	  {
		string filename = "test.txt";
		sbyte[] byteContent = null;
		FileValue variableValue = Variables.fileValue(filename).file(byteContent).mimeType(ContentType.TEXT.ToString()).create();

		MockHistoricVariableUpdateBuilder builder = MockProvider.mockHistoricVariableUpdate();

		HistoricVariableUpdate detailMock = builder.typedValue(variableValue).build();

		when(historicDetailQueryMock.detailId(detailMock.Id)).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.disableCustomObjectDeserialization()).thenReturn(historicDetailQueryMock);
		when(historicDetailQueryMock.singleResult()).thenReturn(detailMock);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_VAR_UPDATE_ID).then().expect().statusCode(Status.OK.StatusCode).and().contentType(ContentType.TEXT).and().body(@is(equalTo(""))).when().get(VARIABLE_INSTANCE_BINARY_DATA_URL);

	  }
	}

}