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
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anySetOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class ProcessInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_INSTANCE_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/process-instance";
	  protected internal static readonly string PROCESS_INSTANCE_COUNT_QUERY_URL = PROCESS_INSTANCE_QUERY_URL + "/count";
	  protected internal ProcessInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockInstanceQuery(createMockInstanceList());
	  }

	  private ProcessInstanceQuery setUpMockInstanceQuery(IList<ProcessInstance> mockedInstances)
	  {
		ProcessInstanceQuery sampleInstanceQuery = mock(typeof(ProcessInstanceQuery));
		when(sampleInstanceQuery.list()).thenReturn(mockedInstances);
		when(sampleInstanceQuery.count()).thenReturn((long) mockedInstances.Count);
		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(sampleInstanceQuery);
		return sampleInstanceQuery;
	  }

	  private IList<ProcessInstance> createMockInstanceList()
	  {
		IList<ProcessInstance> mocks = new List<ProcessInstance>();

		mocks.Add(MockProvider.createMockInstance());
		return mocks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
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
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().get(PROCESS_INSTANCE_QUERY_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(PROCESS_INSTANCE_QUERY_URL);
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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "definitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrieval()
	  public virtual void testInstanceRetrieval()
	  {
		string queryKey = "key";
		Response response = given().queryParam("processDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processDefinitionKey(queryKey);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one process definition returned.", 1, instances.Count);
		Assert.assertNotNull("There should be one process definition returned", instances[0]);

		string returnedInstanceId = from(content).getString("[0].id");
		bool? returnedIsEnded = from(content).getBoolean("[0].ended");
		string returnedDefinitionId = from(content).getString("[0].definitionId");
		string returnedBusinessKey = from(content).getString("[0].businessKey");
		bool? returnedIsSuspended = from(content).getBoolean("[0].suspended");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_ENDED, returnedIsEnded);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY, returnedBusinessKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_IS_SUSPENDED, returnedIsSuspended);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncompleteProcessInstance()
	  public virtual void testIncompleteProcessInstance()
	  {
		UpMockInstanceQuery = createIncompleteMockInstances();
		Response response = expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		string content = response.asString();
		string returnedBusinessKey = from(content).getString("[0].businessKey");
		Assert.assertNull("Should be null, as it is also null in the original process instance on the server.", returnedBusinessKey);
	  }

	  private IList<ProcessInstance> createIncompleteMockInstances()
	  {
		IList<ProcessInstance> mocks = new List<ProcessInstance>();
		ProcessInstance mockInstance = mock(typeof(ProcessInstance));
		when(mockInstance.Id).thenReturn(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);

		mocks.Add(mockInstance);
		return mocks;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariables()
	  public virtual void testAdditionalParametersExcludingVariables()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().queryParams(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).caseInstanceId(queryParameters["caseInstanceId"]);
		verify(mockedQuery).processInstanceBusinessKey(queryParameters["businessKey"]);
		verify(mockedQuery).processInstanceBusinessKeyLike(queryParameters["businessKeyLike"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["processDefinitionKey"]);
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).deploymentId(queryParameters["deploymentId"]);
		verify(mockedQuery).superProcessInstanceId(queryParameters["superProcessInstance"]);
		verify(mockedQuery).subProcessInstanceId(queryParameters["subProcessInstance"]);
		verify(mockedQuery).superCaseInstanceId(queryParameters["superCaseInstance"]);
		verify(mockedQuery).subCaseInstanceId(queryParameters["subCaseInstance"]);
		verify(mockedQuery).suspended();
		verify(mockedQuery).active();
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
			parameters["businessKeyLike"] = "aKeyLike";
			parameters["processDefinitionKey"] = "aProcDefKey";
			parameters["processDefinitionId"] = "aProcDefId";
			parameters["deploymentId"] = "deploymentId";
			parameters["superProcessInstance"] = "aSuperProcInstId";
			parameters["subProcessInstance"] = "aSubProcInstId";
			parameters["superCaseInstance"] = "aSuperCaseInstId";
			parameters["subCaseInstance"] = "aSubCaseInstId";
			parameters["suspended"] = "true";
			parameters["active"] = "true";
			parameters["incidentId"] = "incId";
			parameters["incidentMessage"] = "incMessage";
			parameters["incidentMessageLike"] = "incMessageLike";
			parameters["incidentType"] = "incType";
			parameters["caseInstanceId"] = "aCaseInstanceId";
    
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
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		queryValue = variableName + "_gt_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		queryValue = variableName + "_gteq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_lt_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		queryValue = variableName + "_lteq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		queryValue = variableName + "_like_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueLike(variableName, variableValue);

		queryValue = variableName + "_neq_" + variableValue;
		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);
		verify(mockedQuery).variableValueNotEquals(variableName, variableValue);
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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName1, variableValue1);
		verify(mockedQuery).variableValueNotEquals(variableName2, variableValue2);
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

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDateVariableParameter()
	  public virtual void testDateVariableParameter()
	  {
		string variableName = "varName";
		string variableValue = withTimezone("2014-06-16T10:00:00");
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		DateTime date = DateTimeUtil.parseDate(variableValue);

		verify(mockedQuery).variableValueEquals(variableName, date);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDateVariableParameterAsPost()
	  public virtual void testDateVariableParameterAsPost()
	  {
		string variableName = "varName";
		string variableValue = withTimezone("2014-06-16T10:00:00");

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		DateTime date = DateTimeUtil.parseDate(variableValue);

		verify(mockedQuery).variableValueEquals(variableName, date);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletePostParameters()
	  public virtual void testCompletePostParameters()
	  {
		IDictionary<string, string> queryParameters = CompleteQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).caseInstanceId(queryParameters["caseInstanceId"]);
		verify(mockedQuery).processInstanceBusinessKey(queryParameters["businessKey"]);
		verify(mockedQuery).processInstanceBusinessKeyLike(queryParameters["businessKeyLike"]);
		verify(mockedQuery).processDefinitionKey(queryParameters["processDefinitionKey"]);
		verify(mockedQuery).processDefinitionId(queryParameters["processDefinitionId"]);
		verify(mockedQuery).deploymentId(queryParameters["deploymentId"]);
		verify(mockedQuery).superProcessInstanceId(queryParameters["superProcessInstance"]);
		verify(mockedQuery).subProcessInstanceId(queryParameters["subProcessInstance"]);
		verify(mockedQuery).superCaseInstanceId(queryParameters["superCaseInstance"]);
		verify(mockedQuery).subCaseInstanceId(queryParameters["subCaseInstance"]);
		verify(mockedQuery).suspended();
		verify(mockedQuery).active();
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
		mockedQuery = setUpMockInstanceQuery(createMockProcessInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		assertThat(instances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		mockedQuery = setUpMockInstanceQuery(Arrays.asList(MockProvider.createMockInstance(null)));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockInstanceQuery(createMockProcessInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

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
//ORIGINAL LINE: @Test public void testWithoutTenantIdPostParameter()
	  public virtual void testWithoutTenantIdPostParameter()
	  {
		mockedQuery = setUpMockInstanceQuery(Arrays.asList(MockProvider.createMockInstance(null)));

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

	  private IList<ProcessInstance> createMockProcessInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityIdListParameter()
	  public virtual void testActivityIdListParameter()
	  {
		given().queryParam("activityIdIn", MockProvider.EXAMPLE_ACTIVITY_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).activityIdIn(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityIdListPostParameter()
	  public virtual void testActivityIdListPostParameter()
	  {
		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["activityIdIn"] = MockProvider.EXAMPLE_ACTIVITY_ID_LIST.Split(",", true);

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).activityIdIn(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
		verify(mockedQuery).list();
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

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("businessKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByBusinessKey();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = OrderingBuilder.create().orderBy("definitionKey").desc().orderBy("definitionId").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

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
		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

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
		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(PROCESS_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(PROCESS_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByList()
	  public virtual void testInstanceRetrievalByList()
	  {
		IList<ProcessInstance> mockProcessInstanceList = new List<ProcessInstance>();

		mockProcessInstanceList.Add(MockProvider.createMockInstance());
		mockProcessInstanceList.Add(MockProvider.createAnotherMockInstance());

		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);
		when(instanceQuery.list()).thenReturn(mockProcessInstanceList);

		Response response = given().queryParam("processInstanceIds", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(instanceQuery);
		ISet<string> expectedSet = MockProvider.createMockSetFromList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST);

		inOrder.verify(instanceQuery).processInstanceIds(expectedSet);
		inOrder.verify(instanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be two process definitions returned.", 2, instances.Count);

		string returnedInstanceId1 = from(content).getString("[0].id");
		string returnedInstanceId2 = from(content).getString("[1].id");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId1);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByListAsPost()
	  public virtual void testInstanceRetrievalByListAsPost()
	  {
		IList<ProcessInstance> mockProcessInstanceList = new List<ProcessInstance>();

		mockProcessInstanceList.Add(MockProvider.createMockInstance());
		mockProcessInstanceList.Add(MockProvider.createAnotherMockInstance());

		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);
		when(instanceQuery.list()).thenReturn(mockProcessInstanceList);

		IDictionary<string, ISet<string>> @params = new Dictionary<string, ISet<string>>();
		ISet<string> processInstanceIds = MockProvider.createMockSetFromList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST);
		@params["processInstanceIds"] = processInstanceIds;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(instanceQuery);

		inOrder.verify(instanceQuery).processInstanceIds(processInstanceIds);
		inOrder.verify(instanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be two process definitions returned.", 2, instances.Count);

		string returnedInstanceId1 = from(content).getString("[0].id");
		string returnedInstanceId2 = from(content).getString("[1].id");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId1);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByListWithDuplicate()
	  public virtual void testInstanceRetrievalByListWithDuplicate()
	  {
		IList<ProcessInstance> mockProcessInstanceList = new List<ProcessInstance>();

		mockProcessInstanceList.Add(MockProvider.createMockInstance());
		mockProcessInstanceList.Add(MockProvider.createAnotherMockInstance());

		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(instanceQuery.list()).thenReturn(mockProcessInstanceList);
		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);

		Response response = given().queryParam("processInstanceIds", MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST_WITH_DUP).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(instanceQuery);
		ISet<string> expectedSet = MockProvider.createMockSetFromList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST);

		inOrder.verify(instanceQuery).processInstanceIds(expectedSet);
		inOrder.verify(instanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be two process definitions returned.", 2, instances.Count);

		string returnedInstanceId1 = from(content).getString("[0].id");
		string returnedInstanceId2 = from(content).getString("[1].id");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId1);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByListWithDuplicateAsPost()
	  public virtual void testInstanceRetrievalByListWithDuplicateAsPost()
	  {
		IList<ProcessInstance> mockProcessInstanceList = new List<ProcessInstance>();

		mockProcessInstanceList.Add(MockProvider.createMockInstance());
		mockProcessInstanceList.Add(MockProvider.createAnotherMockInstance());

		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(instanceQuery.list()).thenReturn(mockProcessInstanceList);
		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);

		IDictionary<string, ISet<string>> @params = new Dictionary<string, ISet<string>>();
		ISet<string> processInstanceIds = MockProvider.createMockSetFromList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID_LIST);
		@params["processInstanceIds"] = processInstanceIds;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(instanceQuery);
		inOrder.verify(instanceQuery).processInstanceIds(processInstanceIds);
		inOrder.verify(instanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be two process definitions returned.", 2, instances.Count);

		string returnedInstanceId1 = from(content).getString("[0].id");
		string returnedInstanceId2 = from(content).getString("[1].id");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId1);
		Assert.assertEquals(MockProvider.ANOTHER_EXAMPLE_PROCESS_INSTANCE_ID, returnedInstanceId2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByListWithEmpty()
	  public virtual void testInstanceRetrievalByListWithEmpty()
	  {
		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(instanceQuery.list()).thenReturn(null);
		string expectedExceptionMessage = "Set of process instance ids is empty";
		doThrow(new ProcessEngineException(expectedExceptionMessage)).when(instanceQuery).processInstanceIds(anySetOf(typeof(string)));
		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);

		string emptyList = "";
		given().queryParam("processInstanceIds", emptyList).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo(expectedExceptionMessage)).when().get(PROCESS_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInstanceRetrievalByListWithEmptyAsPost()
	  public virtual void testInstanceRetrievalByListWithEmptyAsPost()
	  {
		ProcessInstanceQuery instanceQuery = mock(typeof(ProcessInstanceQuery));

		when(instanceQuery.list()).thenReturn(null);
		string expectedExceptionMessage = "Set of process instance ids is empty";
		doThrow(new ProcessEngineException(expectedExceptionMessage)).when(instanceQuery).processInstanceIds(anySetOf(typeof(string)));
		when(processEngine.RuntimeService.createProcessInstanceQuery()).thenReturn(instanceQuery);

		IDictionary<string, ISet<string>> @params = new Dictionary<string, ISet<string>>();
		@params["processInstanceIds"] = new HashSet<string>();

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo(expectedExceptionMessage)).when().post(PROCESS_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryRootProcessInstances()
	  public virtual void testQueryRootProcessInstances()
	  {
		given().queryParam("rootProcessInstances", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).rootProcessInstances();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryRootProcessInstancesAsPost()
	  public virtual void testQueryRootProcessInstancesAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["rootProcessInstances"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).rootProcessInstances();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryLeafProcessInstances()
	  public virtual void testQueryLeafProcessInstances()
	  {
		given().queryParam("leafProcessInstances", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).leafProcessInstances();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryLeafProcessInstancesAsPost()
	  public virtual void testQueryLeafProcessInstancesAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["leafProcessInstances"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).leafProcessInstances();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryProcessDefinitionWithoutTenantId()
	  public virtual void testQueryProcessDefinitionWithoutTenantId()
	  {
		given().queryParam("processDefinitionWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).processDefinitionWithoutTenantId();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryProcessDefinitionWithoutTenantIdAsPost()
	  public virtual void testQueryProcessDefinitionWithoutTenantIdAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).processDefinitionWithoutTenantId();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryProcessInstanceWithIncident()
	  public virtual void testQueryProcessInstanceWithIncident()
	  {
		given().queryParam("withIncident", true).then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).withIncident();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryProcessInstanceWithIncidentAsPost()
	  public virtual void testQueryProcessInstanceWithIncidentAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["withIncident"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(PROCESS_INSTANCE_QUERY_URL);

		verify(mockedQuery).withIncident();
	  }
	}

}