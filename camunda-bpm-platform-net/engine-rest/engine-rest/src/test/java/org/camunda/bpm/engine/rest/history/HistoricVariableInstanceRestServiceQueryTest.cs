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
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockHistoricVariableInstanceBuilder = org.camunda.bpm.engine.rest.helper.MockHistoricVariableInstanceBuilder;
	using MockObjectValue = org.camunda.bpm.engine.rest.helper.MockObjectValue;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializableValueType = org.camunda.bpm.engine.variable.type.SerializableValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;

	public class HistoricVariableInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/variable-instance";

	  protected internal static readonly string HISTORIC_VARIABLE_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricVariableInstanceQuery mockedQuery;
	  protected internal HistoricVariableInstance mockInstance;
	  protected internal MockHistoricVariableInstanceBuilder mockInstanceBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockInstanceBuilder = MockProvider.mockHistoricVariableInstance();
		mockInstance = mockInstanceBuilder.build();

		IList<HistoricVariableInstance> mocks = new List<HistoricVariableInstance>();
		mocks.Add(mockInstance);

		mockedQuery = setUpMockHistoricVariableInstanceQuery(mocks);
	  }

	  private HistoricVariableInstanceQuery setUpMockHistoricVariableInstanceQuery(IList<HistoricVariableInstance> mockedHistoricVariableInstances)
	  {

		HistoricVariableInstanceQuery mockedHistoricVariableInstanceQuery = mock(typeof(HistoricVariableInstanceQuery));
		when(mockedHistoricVariableInstanceQuery.list()).thenReturn(mockedHistoricVariableInstances);
		when(mockedHistoricVariableInstanceQuery.count()).thenReturn((long) mockedHistoricVariableInstances.Count);

		when(processEngine.HistoryService.createHistoricVariableInstanceQuery()).thenReturn(mockedHistoricVariableInstanceQuery);

		return mockedHistoricVariableInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();

		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryDisableObjectDeserialization()
	  public virtual void testNoParametersQueryDisableObjectDeserialization()
	  {
		given().queryParam("deserializeValues", false).expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();

		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPostDisableObjectDeserialization()
	  public virtual void testNoParametersQueryAsPostDisableObjectDeserialization()
	  {
		given().queryParam("deserializeValues", false).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("instanceId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "instanceId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);

		executeAndVerifySorting("instanceId", "asc", Status.OK);

		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);

		executeAndVerifySorting("variableName", "desc", Status.OK);

		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = OrderingBuilder.create().orderBy("instanceId").desc().orderBy("variableName").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_VARIABLE_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().body("count", equalTo(1)).when().post(HISTORIC_VARIABLE_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableNameLikeQuery()
	  public virtual void testVariableNameLikeQuery()
	  {
		string variableNameLike = "aVariableNameLike";

		given().queryParam("variableNameLike", variableNameLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).variableNameLike(variableNameLike);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByVariableTypeIn()
	  public virtual void testHistoricVariableQueryByVariableTypeIn()
	  {
		string aVariableType = "string";
		string anotherVariableType = "integer";

		given().queryParam("variableTypeIn", aVariableType + "," + anotherVariableType).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableTypeIn(aVariableType, anotherVariableType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByVariableTypeInAsPost()
	  public virtual void testHistoricVariableQueryByVariableTypeInAsPost()
	  {
		string aVariableType = "string";
		string anotherVariableType = "integer";

		IList<string> variableTypeIn = new List<string>();
		variableTypeIn.Add(aVariableType);
		variableTypeIn.Add(anotherVariableType);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variableTypeIn"] = variableTypeIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableTypeIn(aVariableType, anotherVariableType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricVariableQuery()
	  public virtual void testSimpleHistoricVariableQuery()
	  {
		string processInstanceId = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).and().body("size()", @is(1)).body("[0].id", equalTo(mockInstanceBuilder.Id)).body("[0].name", equalTo(mockInstanceBuilder.Name)).body("[0].type", equalTo(VariableTypeHelper.toExpectedValueTypeName(mockInstanceBuilder.TypedValue.Type))).body("[0].value", equalTo(mockInstanceBuilder.Value)).body("[0].processDefinitionKey", equalTo(mockInstanceBuilder.ProcessDefinitionKey)).body("[0].processDefinitionId", equalTo(mockInstanceBuilder.ProcessDefinitionId)).body("[0].processInstanceId", equalTo(mockInstanceBuilder.ProcessInstanceId)).body("[0].executionId", equalTo(mockInstanceBuilder.ExecutionId)).body("[0].errorMessage", equalTo(mockInstanceBuilder.ErrorMessage)).body("[0].activityInstanceId", equalTo(mockInstanceBuilder.ActivityInstanceId)).body("[0].caseDefinitionKey", equalTo(mockInstanceBuilder.CaseDefinitionKey)).body("[0].caseDefinitionId", equalTo(mockInstanceBuilder.CaseDefinitionId)).body("[0].caseInstanceId", equalTo(mockInstanceBuilder.CaseInstanceId)).body("[0].caseExecutionId", equalTo(mockInstanceBuilder.CaseExecutionId)).body("[0].taskId", equalTo(mockInstanceBuilder.TaskId)).body("[0].tenantId", equalTo(mockInstanceBuilder.TenantId)).body("[0].createTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_CREATE_TIME)).body("[0].removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_VARIABLE_INSTANCE_REMOVAL_TIME)).body("[0].rootProcessInstanceId", equalTo(mockInstanceBuilder.RootProcessInstanceId)).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processInstanceId(processInstanceId);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSerializableVariableInstanceRetrieval()
	  public virtual void testSerializableVariableInstanceRetrieval()
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance().typedValue(MockObjectValue.fromObjectValue(Variables.objectValue("a serialized value").serializationDataFormat(Variables.SerializationDataFormats.JAVA).create()).objectTypeName(typeof(string).FullName));

		IList<HistoricVariableInstance> mockInstances = new List<HistoricVariableInstance>();
		mockInstances.Add(builder.build());

		mockedQuery = setUpMockHistoricVariableInstanceQuery(mockInstances);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		given().then().expect().statusCode(Status.OK.StatusCode).and().body("[0].type", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT))).body("[0].value", equalTo("a serialized value")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo(typeof(string).FullName)).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo(Variables.SerializationDataFormats.JAVA.Name)).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		// should not resolve custom objects but existing API requires it
	//  verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSpinVariableInstanceRetrieval()
	  public virtual void testSpinVariableInstanceRetrieval()
	  {
		MockHistoricVariableInstanceBuilder builder = MockProvider.mockHistoricVariableInstance().typedValue(Variables.serializedObjectValue("aSpinSerializedValue").serializationDataFormat("aDataFormat").objectTypeName("aRootType").create());

		IList<HistoricVariableInstance> mockInstances = new List<HistoricVariableInstance>();
		mockInstances.Add(builder.build());

		mockedQuery = setUpMockHistoricVariableInstanceQuery(mockInstances);

		given().then().expect().statusCode(Status.OK.StatusCode).and().body("size()", @is(1)).body("[0].type", equalTo(VariableTypeHelper.toExpectedValueTypeName(ValueType.OBJECT))).body("[0].value", equalTo("aSpinSerializedValue")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_OBJECT_TYPE_NAME, equalTo("aRootType")).body("[0].valueInfo." + SerializableValueType.VALUE_INFO_SERIALIZATION_DATA_FORMAT, equalTo("aDataFormat")).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariables()
	  public virtual void testAdditionalParametersExcludingVariables()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
		verify(mockedQuery).list();
	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["processInstanceId"] = MockProvider.EXAMPLE_VARIABLE_INSTANCE_PROC_INST_ID;
			parameters["variableName"] = MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME;
			parameters["variableValue"] = MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value;
			return parameters;
		  }
	  }

	  private void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockedQuery).variableName(stringQueryParameters["variableName"]);
		verify(mockedQuery).variableValueEquals(stringQueryParameters["variableName"], stringQueryParameters["variableValue"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableNameAndValueQuery()
	  public virtual void testVariableNameAndValueQuery()
	  {
		string variableName = MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME;
		string variableValue = MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value;

		given().queryParam("variableName", variableName).queryParam("variableValue", variableValue).then().expect().statusCode(Status.OK.StatusCode).and().body("size()", @is(1)).body("[0].name", equalTo(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME)).body("[0].value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).variableValueEquals(variableName, variableValue);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableValueQuery_BadRequest()
	  public virtual void testVariableValueQuery_BadRequest()
	  {
		given().queryParam("variableValue", MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single variable value parameter specified: variable name and value are required to be able to query after a specific variable value.")).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByExecutionIdsAndTaskIds()
	  public virtual void testHistoricVariableQueryByExecutionIdsAndTaskIds()
	  {
		  string anExecutionId = "anExecutionId";
		  string anotherExecutionId = "anotherExecutionId";

		  string aTaskId = "aTaskId";
		  string anotherTaskId = "anotherTaskId";

		  given().queryParam("executionIdIn", anExecutionId + "," + anotherExecutionId).queryParam("taskIdIn", aTaskId + "," + anotherTaskId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		  verify(mockedQuery).taskIdIn(aTaskId, anotherTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByExecutionIdsAndTaskIdsAsPost()
	  public virtual void testHistoricVariableQueryByExecutionIdsAndTaskIdsAsPost()
	  {
		string anExecutionId = "anExecutionId";
		string anotherExecutionId = "anotherExecutionId";

		IList<string> executionIdIn = new List<string>();
		executionIdIn.Add(anExecutionId);
		executionIdIn.Add(anotherExecutionId);

		string aTaskId = "aTaskId";
		string anotherTaskId = "anotherTaskId";

		IList<string> taskIdIn = new List<string>();
		taskIdIn.Add(aTaskId);
		taskIdIn.Add(anotherTaskId);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["executionIdIn"] = executionIdIn;
		json["taskIdIn"] = taskIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		verify(mockedQuery).taskIdIn(aTaskId, anotherTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessInstanceIdIn()
	  public virtual void testHistoricVariableQueryByProcessInstanceIdIn()
	  {
		string aProcessInstanceId = "aProcessInstanceId";
		string anotherProcessInstanceId = "anotherProcessInstanceId";

		given().queryParam("processInstanceIdIn", aProcessInstanceId + "," + anotherProcessInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessInstanceIdInAsPOST()
	  public virtual void testHistoricVariableQueryByProcessInstanceIdInAsPOST()
	  {
		string aProcessInstanceId = "aProcessInstanceId";
		string anotherProcessInstanceId = "anotherProcessInstanceId";

		IList<string> processInstanceIdIn = new List<string>();
		processInstanceIdIn.Add(aProcessInstanceId);
		processInstanceIdIn.Add(anotherProcessInstanceId);
		processInstanceIdIn.Add(null);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIdIn"] = processInstanceIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId,null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByActivityInstanceIds()
	  public virtual void testHistoricVariableQueryByActivityInstanceIds()
	  {
		  string anActivityInstanceId = "anActivityInstanceId";
		  string anotherActivityInstanceId = "anotherActivityInstanceId";

		  given().queryParam("activityInstanceIdIn", anActivityInstanceId + "," + anotherActivityInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).activityInstanceIdIn(anActivityInstanceId, anotherActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByActivityInstanceIdsAsPost()
	  public virtual void testHistoricVariableQueryByActivityInstanceIdsAsPost()
	  {
		string anActivityInstanceId = "anActivityInstanceId";
		string anotherActivityInstanceId = "anotherActivityInstanceId";

		IList<string> activityInstanceIdIn = new List<string>();
		activityInstanceIdIn.Add(anActivityInstanceId);
		activityInstanceIdIn.Add(anotherActivityInstanceId);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["activityInstanceIdIn"] = activityInstanceIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activityInstanceIdIn(anActivityInstanceId, anotherActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseInstanceId()
	  public virtual void testHistoricVariableQueryByCaseInstanceId()
	  {

		given().queryParam("caseInstanceId", MockProvider.EXAMPLE_CASE_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseInstanceIdAsPost()
	  public virtual void testHistoricVariableQueryByCaseInstanceIdAsPost()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceId"] = MockProvider.EXAMPLE_CASE_INSTANCE_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseExecutionIds()
	  public virtual void testHistoricVariableQueryByCaseExecutionIds()
	  {

		string caseExecutionIds = MockProvider.EXAMPLE_CASE_EXECUTION_ID + "," + MockProvider.ANOTHER_EXAMPLE_CASE_EXECUTION_ID;

		given().queryParam("caseExecutionIdIn", caseExecutionIds).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseExecutionIdIn(MockProvider.EXAMPLE_CASE_EXECUTION_ID, MockProvider.ANOTHER_EXAMPLE_CASE_EXECUTION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseExecutionIdsAsPost()
	  public virtual void testHistoricVariableQueryByCaseExecutionIdsAsPost()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseExecutionIdIn"] = Arrays.asList(MockProvider.EXAMPLE_CASE_EXECUTION_ID, MockProvider.ANOTHER_EXAMPLE_CASE_EXECUTION_ID);

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseExecutionIdIn(MockProvider.EXAMPLE_CASE_EXECUTION_ID, MockProvider.ANOTHER_EXAMPLE_CASE_EXECUTION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricVariableInstanceQuery(createMockHistoricVariableInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockHistoricVariableInstanceQuery(createMockHistoricVariableInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseActivityIds()
	  public virtual void testHistoricVariableQueryByCaseActivityIds()
	  {

		string caseExecutionIds = MockProvider.EXAMPLE_CASE_ACTIVITY_ID + "," + MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID;

		given().queryParam("caseActivityIdIn", caseExecutionIds).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityIdIn(MockProvider.EXAMPLE_CASE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByCaseActivityIdsAsPost()
	  public virtual void testHistoricVariableQueryByCaseActivityIdsAsPost()
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseActivityIdIn"] = Arrays.asList(MockProvider.EXAMPLE_CASE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID);

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityIdIn(MockProvider.EXAMPLE_CASE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeDeletedVariables()
	  public virtual void testIncludeDeletedVariables()
	  {
		when(mockedQuery.includeDeleted()).thenReturn(mockedQuery);

		given().queryParam("includeDeleted", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).includeDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessDefinitionIdAsPost()
	  public virtual void testHistoricVariableQueryByProcessDefinitionIdAsPost()
	  {
		when(mockedQuery.processDefinitionId(anyString())).thenReturn(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessDefinitionId()
	  public virtual void testHistoricVariableQueryByProcessDefinitionId()
	  {
		when(mockedQuery.processDefinitionId(anyString())).thenReturn(mockedQuery);

		given().queryParameter("processDefinitionId", MockProvider.EXAMPLE_PROCESS_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessDefinitionKeyAsPost()
	  public virtual void testHistoricVariableQueryByProcessDefinitionKeyAsPost()
	  {
		when(mockedQuery.processDefinitionKey(anyString())).thenReturn(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricVariableQueryByProcessDefinitionKey()
	  public virtual void testHistoricVariableQueryByProcessDefinitionKey()
	  {
		when(mockedQuery.processDefinitionKey(anyString())).thenReturn(mockedQuery);

		given().queryParameter("processDefinitionKey", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_VARIABLE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
	  }

	  private IList<HistoricVariableInstance> createMockHistoricVariableInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.mockHistoricVariableInstance(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockHistoricVariableInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
	  }
	}

}