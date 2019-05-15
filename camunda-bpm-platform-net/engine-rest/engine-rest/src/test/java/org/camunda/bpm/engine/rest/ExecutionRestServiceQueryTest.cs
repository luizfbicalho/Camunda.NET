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
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ExecutionQuery = org.camunda.bpm.engine.runtime.ExecutionQuery;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class ExecutionRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string EXECUTION_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/execution";
	  protected internal static readonly string EXECUTION_COUNT_QUERY_URL = EXECUTION_QUERY_URL + "/count";

	  private ExecutionQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockExecutionQuery(createMockExecutionList());
	  }

	  private ExecutionQuery setUpMockExecutionQuery(IList<Execution> mockedExecutions)
	  {
		ExecutionQuery sampleExecutionQuery = mock(typeof(ExecutionQuery));
		when(sampleExecutionQuery.list()).thenReturn(mockedExecutions);
		when(sampleExecutionQuery.count()).thenReturn((long) mockedExecutions.Count);
		when(processEngine.RuntimeService.createExecutionQuery()).thenReturn(sampleExecutionQuery);
		return sampleExecutionQuery;
	  }

	  private IList<Execution> createMockExecutionList()
	  {
		IList<Execution> mocks = new List<Execution>();

		mocks.Add(MockProvider.createMockExecution());
		return mocks;
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
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().get(EXECUTION_QUERY_URL);

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid process variable comparator specified: " + invalidComparator)).when().get(EXECUTION_QUERY_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(EXECUTION_QUERY_URL);

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(EXECUTION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("definitionId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(EXECUTION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "definitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(EXECUTION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(EXECUTION_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutionRetrieval()
	  public virtual void testExecutionRetrieval()
	  {
		string queryKey = "key";
		Response response = given().queryParam("processDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processDefinitionKey(queryKey);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		Assert.assertEquals("There should be one execution returned.", 1, executions.Count);
		Assert.assertNotNull("There should be one execution returned", executions[0]);

		string returnedExecutionId = from(content).getString("[0].id");
		bool? returnedIsEnded = from(content).getBoolean("[0].ended");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_IS_ENDED, returnedIsEnded);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncompleteExecution()
	  public virtual void testIncompleteExecution()
	  {
		UpMockExecutionQuery = createIncompleteMockExecutions();
		Response response = expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		string content = response.asString();
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		Assert.assertNull("Should be null, as it is also null in the original execution on the server.", returnedProcessInstanceId);
	  }

	  private IList<Execution> createIncompleteMockExecutions()
	  {
		IList<Execution> mocks = new List<Execution>();
		Execution mockExecution = mock(typeof(Execution));
		when(mockExecution.Id).thenReturn(MockProvider.EXAMPLE_EXECUTION_ID);

		mocks.Add(mockExecution);
		return mocks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariables()
	  public virtual void testAdditionalParametersExcludingVariables()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).processInstanceBusinessKey(queryParameters["businessKey"]);
		verify(mockedQuery).processInstanceId(queryParameters["processInstanceId"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["processDefinitionKey"]);
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).activityId(queryParameters["activityId"]);
		verify(mockedQuery).signalEventSubscriptionName(queryParameters["signalEventSubscriptionName"]);
		verify(mockedQuery).messageEventSubscriptionName(queryParameters["messageEventSubscriptionName"]);
		verify(mockedQuery).active();
		verify(mockedQuery).suspended();
		verify(mockedQuery).incidentId(queryParameters["incidentId"]);
		verify(mockedQuery).incidentMessage(queryParameters["incidentMessage"]);
		verify(mockedQuery).incidentMessageLike(queryParameters["incidentMessageLike"]);
		verify(mockedQuery).incidentType(queryParameters["incidentType"]);

		verify(mockedQuery).list();
	  }

	  private IDictionary<string, string> CompleteQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["businessKey"] = "aBusinessKey";
			parameters["processInstanceId"] = "aProcInstId";
			parameters["processDefinitionKey"] = "aProcDefKey";
			parameters["processDefinitionId"] = "aProcDefId";
			parameters["activityId"] = "anActivityId";
			parameters["signalEventSubscriptionName"] = "anEventName";
			parameters["messageEventSubscriptionName"] = "aMessageName";
			parameters["suspended"] = "true";
			parameters["active"] = "true";
			parameters["incidentId"] = "incId";
			parameters["incidentType"] = "incType";
			parameters["incidentMessage"] = "incMessage";
			parameters["incidentMessageLike"] = "incMessageLike";
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableParameters()
	  public virtual void testVariableParameters()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		queryValue = variableName + "_gt_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		queryValue = variableName + "_gteq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_lt_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		queryValue = variableName + "_lteq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_like_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueLike(variableName, variableValue);

		queryValue = variableName + "_neq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).variableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableParameters()
	  public virtual void testProcessVariableParameters()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;
		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).processVariableValueEquals(variableName, variableValue);

		queryValue = variableName + "_neq_" + variableValue;
		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);
		verify(mockedQuery).processVariableValueNotEquals(variableName, variableValue);
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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName1, variableValue1);
		verify(mockedQuery).variableValueNotEquals(variableName2, variableValue2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleProcessVariableParameters()
	  public virtual void testMultipleProcessVariableParameters()
	  {
		string variableName1 = "varName";
		string variableValue1 = "varValue";
		string variableParameter1 = variableName1 + "_eq_" + variableValue1;

		string variableName2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";
		string variableParameter2 = variableName2 + "_neq_" + variableValue2;

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).processVariableValueEquals(variableName1, variableValue1);
		verify(mockedQuery).processVariableValueNotEquals(variableName2, variableValue2);
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
		json["variables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTION_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleProcessVariableParametersAsPost()
	  public virtual void testMultipleProcessVariableParametersAsPost()
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
		json["processVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTION_QUERY_URL);

		verify(mockedQuery).processVariableValueEquals(variableName, variableValue);
		verify(mockedQuery).processVariableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletePostParameters()
	  public virtual void testCompletePostParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(EXECUTION_QUERY_URL);

		verify(mockedQuery).processInstanceBusinessKey(queryParameters["businessKey"]);
		verify(mockedQuery).processInstanceId(queryParameters["processInstanceId"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["processDefinitionKey"]);
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).activityId(queryParameters["activityId"]);
		verify(mockedQuery).signalEventSubscriptionName(queryParameters["signalEventSubscriptionName"]);
		verify(mockedQuery).messageEventSubscriptionName(queryParameters["messageEventSubscriptionName"]);
		verify(mockedQuery).active();
		verify(mockedQuery).suspended();
		verify(mockedQuery).incidentId(queryParameters["incidentId"]);
		verify(mockedQuery).incidentMessage(queryParameters["incidentMessage"]);
		verify(mockedQuery).incidentMessageLike(queryParameters["incidentMessageLike"]);
		verify(mockedQuery).incidentType(queryParameters["incidentType"]);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockExecutionQuery(createMockExecutionsTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

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
		mockedQuery = setUpMockExecutionQuery(createMockExecutionsTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(EXECUTION_QUERY_URL);

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

	  private IList<Execution> createMockExecutionsTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockExecution(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockExecution(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
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
		executeAndVerifySorting("definitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = OrderingBuilder.create().orderBy("definitionKey").desc().orderBy("instanceId").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTION_QUERY_URL);

		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

	  /// <summary>
	  /// If parameter "firstResult" is missing, we expect 0 as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

	  /// <summary>
	  /// If parameter "maxResults" is missing, we expect Integer.MAX_VALUE as default.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTION_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(EXECUTION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(EXECUTION_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }
	}

}