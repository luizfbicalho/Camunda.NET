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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
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



	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using MockVariableInstanceBuilder = org.camunda.bpm.engine.rest.helper.MockVariableInstanceBuilder;
	using VariableTypeHelper = org.camunda.bpm.engine.rest.helper.VariableTypeHelper;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class VariableInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string VARIABLE_INSTANCE_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/variable-instance";
	  protected internal static readonly string VARIABLE_INSTANCE_COUNT_QUERY_URL = VARIABLE_INSTANCE_QUERY_URL + "/count";

	  protected internal VariableInstanceQuery mockedQuery;
	  protected internal VariableInstance mockInstance;
	  protected internal MockVariableInstanceBuilder mockInstanceBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockInstanceBuilder = MockProvider.mockVariableInstance();
		mockInstance = mockInstanceBuilder.build();

		mockedQuery = setUpMockVariableInstanceQuery(createMockVariableInstanceList(mockInstance));
	  }

	  private VariableInstanceQuery setUpMockVariableInstanceQuery(IList<VariableInstance> mockedInstances)
	  {
		VariableInstanceQuery sampleInstanceQuery = mock(typeof(VariableInstanceQuery));

		when(sampleInstanceQuery.list()).thenReturn(mockedInstances);
		when(sampleInstanceQuery.count()).thenReturn((long) mockedInstances.Count);
		when(processEngine.RuntimeService.createVariableInstanceQuery()).thenReturn(sampleInstanceQuery);

		return sampleInstanceQuery;
	  }

	  protected internal virtual IList<VariableInstance> createMockVariableInstanceList(VariableInstance mockInstance)
	  {
		IList<VariableInstance> mocks = new List<VariableInstance>();

		mocks.Add(mockInstance);
		return mocks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryDisableObjectDeserialization()
	  public virtual void testNoParametersQueryDisableObjectDeserialization()
	  {
		given().queryParam("deserializeValues", false).expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPostDisableObjectDeserialization()
	  public virtual void testNoParametersQueryAsPostDisableObjectDeserialization()
	  {
		given().queryParam("deserializeValues", false).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();
		verify(mockedQuery).disableCustomObjectDeserialization();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidVariableRequests()
	  public virtual void testInvalidVariableRequests()
	  {
		// invalid comparator
		string invalidComparator = "anInvalidComparator";
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_" + invalidComparator + "_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().get(VARIABLE_INSTANCE_QUERY_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(VARIABLE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("variableName", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "variableName").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("variableType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByVariableType();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityInstanceId();
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
		json["sorting"] = OrderingBuilder.create().orderBy("variableName").desc().orderBy("activityInstanceId").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		inOrder.verify(mockedQuery).orderByVariableName();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByActivityInstanceId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

	  /// <summary>
	  /// If parameter "firstResult" is missing, we expect 0 as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

	  /// <summary>
	  /// If parameter "maxResults" is missing, we expect Integer.MAX_VALUE as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableInstanceRetrieval()
	  public virtual void testVariableInstanceRetrieval()
	  {
		string queryVariableName = "aVariableInstanceName";
		Response response = given().queryParam("variableName", queryVariableName).then().expect().statusCode(Status.OK.StatusCode).and().body("size()", @is(1)).body("[0].id", equalTo(mockInstanceBuilder.Id)).body("[0].name", equalTo(mockInstanceBuilder.Name)).body("[0].type", equalTo(VariableTypeHelper.toExpectedValueTypeName(mockInstanceBuilder.TypedValue.Type))).body("[0].value", equalTo(mockInstanceBuilder.Value)).body("[0].processInstanceId", equalTo(mockInstanceBuilder.ProcessInstanceId)).body("[0].executionId", equalTo(mockInstanceBuilder.ExecutionId)).body("[0].caseInstanceId", equalTo(mockInstanceBuilder.CaseInstanceId)).body("[0].caseExecutionId", equalTo(mockInstanceBuilder.CaseExecutionId)).body("[0].taskId", equalTo(mockInstanceBuilder.TaskId)).body("[0].activityInstanceId", equalTo(mockInstanceBuilder.ActivityInstanceId)).body("[0].tenantId", equalTo(mockInstanceBuilder.TenantId)).body("[0].errorMessage", equalTo(mockInstanceBuilder.ErrorMessage)).body("[0].serializedValue", nullValue()).when().get(VARIABLE_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).variableName(queryVariableName);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> variables = from(content).getList("");
		Assert.assertEquals("There should be one variable instance returned.", 1, variables.Count);
		Assert.assertNotNull("There should be one variable instance returned", variables[0]);

		verify(mockedQuery).disableBinaryFetching();
		// requirement to not break existing API; should be:
		// verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableInstanceRetrievalAsPost()
	  public virtual void testVariableInstanceRetrievalAsPost()
	  {
		string queryVariableName = "aVariableInstanceName";
		IDictionary<string, string> queryParameter = new Dictionary<string, string>();
		queryParameter["variableName"] = queryVariableName;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameter).then().expect().statusCode(Status.OK.StatusCode).and().body("size()", @is(1)).body("[0].id", equalTo(mockInstanceBuilder.Id)).body("[0].name", equalTo(mockInstanceBuilder.Name)).body("[0].type", equalTo(VariableTypeHelper.toExpectedValueTypeName(mockInstanceBuilder.TypedValue.Type))).body("[0].value", equalTo(mockInstanceBuilder.TypedValue.Value)).body("[0].processInstanceId", equalTo(mockInstanceBuilder.ProcessInstanceId)).body("[0].executionId", equalTo(mockInstanceBuilder.ExecutionId)).body("[0].caseInstanceId", equalTo(mockInstanceBuilder.CaseInstanceId)).body("[0].caseExecutionId", equalTo(mockInstanceBuilder.CaseExecutionId)).body("[0].taskId", equalTo(mockInstanceBuilder.TaskId)).body("[0].activityInstanceId", equalTo(mockInstanceBuilder.ActivityInstanceId)).body("[0].tenantId", equalTo(mockInstanceBuilder.TenantId)).body("[0].errorMessage", equalTo(mockInstanceBuilder.ErrorMessage)).body("[0].serializedValue", nullValue()).when().post(VARIABLE_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).variableName(queryVariableName);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> variables = from(content).getList("");
		Assert.assertEquals("There should be one process definition returned.", 1, variables.Count);
		Assert.assertNotNull("There should be one process definition returned", variables[0]);

		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariableValues()
	  public virtual void testAdditionalParametersExcludingVariableValues()
	  {
		IDictionary<string, string> queryParameters = new Dictionary<string, string>();

		queryParameters["variableName"] = "aVariableName";
		queryParameters["variableNameLike"] = "aVariableNameLike";
		queryParameters["executionIdIn"] = "anExecutionId";
		queryParameters["processInstanceIdIn"] = "aProcessInstanceId";
		queryParameters["caseExecutionIdIn"] = "aCaseExecutionId";
		queryParameters["caseInstanceIdIn"] = "aCaseInstanceId";
		queryParameters["taskIdIn"] = "aTaskId";
		queryParameters["variableScopeIdIn"] = "aVariableScopeId";
		queryParameters["activityInstanceIdIn"] = "anActivityInstanceId";
		queryParameters["tenantIdIn"] = "anTenantId";

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableName(queryParameters["variableName"]);
		verify(mockedQuery).variableNameLike(queryParameters["variableNameLike"]);
		verify(mockedQuery).processInstanceIdIn(queryParameters["processInstanceIdIn"]);
		verify(mockedQuery).executionIdIn(queryParameters["executionIdIn"]);
		verify(mockedQuery).caseInstanceIdIn(queryParameters["caseInstanceIdIn"]);
		verify(mockedQuery).caseExecutionIdIn(queryParameters["caseExecutionIdIn"]);
		verify(mockedQuery).taskIdIn(queryParameters["taskIdIn"]);
		verify(mockedQuery).variableScopeIdIn(queryParameters["variableScopeIdIn"]);
		verify(mockedQuery).activityInstanceIdIn(queryParameters["activityInstanceIdIn"]);
		verify(mockedQuery).tenantIdIn(queryParameters["tenantIdIn"]);
		verify(mockedQuery).list();

		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariableValuesAsPost()
	  public virtual void testAdditionalParametersExcludingVariableValuesAsPost()
	  {
		string aVariableName = "aVariableName";
		string aVariableNameLike = "aVariableNameLike";
		string aProcessInstanceId = "aProcessInstanceId";
		string anExecutionId = "anExecutionId";
		string aTaskId = "aTaskId";
		string aVariableScopeId = "aVariableScopeId";
		string anActivityInstanceId = "anActivityInstanceId";
		string aCaseInstanceId = "aCaseInstanceId";
		string aCaseExecutionId = "aCaseExecutionId";
		string aTenantId = "aTenantId";

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();

		queryParameters["variableName"] = aVariableName;
		queryParameters["variableNameLike"] = aVariableNameLike;

		IList<string> executionIdIn = new List<string>();
		executionIdIn.Add(anExecutionId);
		queryParameters["executionIdIn"] = executionIdIn;

		IList<string> processInstanceIdIn = new List<string>();
		processInstanceIdIn.Add(aProcessInstanceId);
		queryParameters["processInstanceIdIn"] = processInstanceIdIn;

		IList<string> caseExecutionIdIn = new List<string>();
		caseExecutionIdIn.Add(aCaseExecutionId);
		queryParameters["caseExecutionIdIn"] = caseExecutionIdIn;

		IList<string> caseInstanceIdIn = new List<string>();
		caseInstanceIdIn.Add(aCaseInstanceId);
		queryParameters["caseInstanceIdIn"] = caseInstanceIdIn;

		IList<string> taskIdIn = new List<string>();
		taskIdIn.Add(aTaskId);
		queryParameters["taskIdIn"] = taskIdIn;

		IList<string> variableScopeIdIn = new List<string>();
		variableScopeIdIn.Add(aVariableScopeId);
		queryParameters["variableScopeIdIn"] = variableScopeIdIn;

		IList<string> activityInstanceIdIn = new List<string>();
		activityInstanceIdIn.Add(anActivityInstanceId);
		queryParameters["activityInstanceIdIn"] = activityInstanceIdIn;

		IList<string> tenantIdIn = new List<string>();
		tenantIdIn.Add(aTenantId);
		queryParameters["tenantIdIn"] = tenantIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableName(aVariableName);
		verify(mockedQuery).variableNameLike(aVariableNameLike);
		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId);
		verify(mockedQuery).executionIdIn(anExecutionId);
		verify(mockedQuery).taskIdIn(aTaskId);
		verify(mockedQuery).variableScopeIdIn(aVariableScopeId);
		verify(mockedQuery).activityInstanceIdIn(anActivityInstanceId);
		verify(mockedQuery).tenantIdIn(aTenantId);
		verify(mockedQuery).list();
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(mockedQuery).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableParameters()
	  public virtual void testVariableParameters()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		queryValue = variableName + "_gt_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		queryValue = variableName + "_gteq_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_lt_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		queryValue = variableName + "_lteq_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_like_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLike(variableName, variableValue);

		queryValue = variableName + "_neq_" + variableValue;
		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueNotEquals(variableName, variableValue);
		verify(mockedQuery, times(7)).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(mockedQuery, times(7)).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleVariableParameters()
	  public virtual void testMultipleVariableParameters()
	  {
		string variableName1 = "varName";
		string variableValue1 = "varValue";
		string variableParameter1 = variableName1 + "_eq_" + variableValue1;

		string variableName2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";
		string variableParameter2 = variableName2 + "_neq_" + variableValue2;

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam("variableValues", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName1, variableValue1);
		verify(mockedQuery).variableValueNotEquals(variableName2, variableValue2);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleVariableParametersAsPost()
	  public virtual void testMultipleVariableParametersAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string anotherVariableName = "anotherVarName";
		int? anotherVariableValue = 30;

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IDictionary<string, object> anotherVariableJson = new Dictionary<string, object>();
		anotherVariableJson["name"] = anotherVariableName;
		anotherVariableJson["operator"] = "neq";
		anotherVariableJson["value"] = anotherVariableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);
		variables.Add(anotherVariableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variableValues"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParameters()
	  public virtual void testMultipleParameters()
	  {
		string aProcessInstanceId = "aProcessInstanceId";
		string anotherProcessInstanceId = "anotherProcessInstanceId";

		string anExecutionId = "anExecutionId";
		string anotherExecutionId = "anotherExecutionId";

		string aTaskId = "aTaskId";
		string anotherTaskId = "anotherTaskId";

		string aVariableScopeId = "aVariableScopeId";
		string anotherVariableScopeId = "anotherVariableScopeId";

		string anActivityInstanceId = "anActivityInstanceId";
		string anotherActivityInstanceId = "anotherActivityInstanceId";

		given().queryParam("processInstanceIdIn", aProcessInstanceId + "," + anotherProcessInstanceId).queryParam("executionIdIn", anExecutionId + "," + anotherExecutionId).queryParam("taskIdIn", aTaskId + "," + anotherTaskId).queryParam("variableScopeIdIn", aVariableScopeId + "," + anotherVariableScopeId).queryParam("activityInstanceIdIn", anActivityInstanceId + "," + anotherActivityInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId);
		verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		verify(mockedQuery).taskIdIn(aTaskId, anotherTaskId);
		verify(mockedQuery).variableScopeIdIn(aVariableScopeId, anotherVariableScopeId);
		verify(mockedQuery).activityInstanceIdIn(anActivityInstanceId, anotherActivityInstanceId);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParametersAsPost()
	  public virtual void testMultipleParametersAsPost()
	  {
		string aProcessInstanceId = "aProcessInstanceId";
		string anotherProcessInstanceId = "anotherProcessInstanceId";

		IList<string> processDefinitionIdIn = new List<string>();
		processDefinitionIdIn.Add(aProcessInstanceId);
		processDefinitionIdIn.Add(anotherProcessInstanceId);

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

		string aVariableScopeId = "aVariableScopeId";
		string anotherVariableScopeId = "anotherVariableScopeId";

		IList<string> variableScopeIdIn = new List<string>();
		variableScopeIdIn.Add(aVariableScopeId);
		variableScopeIdIn.Add(anotherVariableScopeId);

		string anActivityInstanceId = "anActivityInstanceId";
		string anotherActivityInstanceId = "anotherActivityInstanceId";

		IList<string> activityInstanceIdIn = new List<string>();
		activityInstanceIdIn.Add(anActivityInstanceId);
		activityInstanceIdIn.Add(anotherActivityInstanceId);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceIdIn"] = processDefinitionIdIn;
		json["executionIdIn"] = executionIdIn;
		json["taskIdIn"] = taskIdIn;
		json["variableScopeIdIn"] = variableScopeIdIn;
		json["activityInstanceIdIn"] = activityInstanceIdIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(VARIABLE_INSTANCE_QUERY_URL);

		verify(mockedQuery).processInstanceIdIn(aProcessInstanceId, anotherProcessInstanceId);
		verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		verify(mockedQuery).taskIdIn(aTaskId, anotherTaskId);
		verify(mockedQuery).variableScopeIdIn(aVariableScopeId, anotherVariableScopeId);
		verify(mockedQuery).activityInstanceIdIn(anActivityInstanceId, anotherActivityInstanceId);
		verify(mockedQuery).disableBinaryFetching();

		// requirement to not break existing API; should be:
		// verify(variableInstanceQueryMock).disableCustomObjectDeserialization();
		verify(mockedQuery, never()).disableCustomObjectDeserialization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(VARIABLE_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(VARIABLE_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

	}

}