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



	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using CaseInstanceQuery = org.camunda.bpm.engine.runtime.CaseInstanceQuery;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using Response = io.restassured.response.Response;

	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CaseInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string CASE_INSTANCE_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/case-instance";
	  protected internal static readonly string CASE_INSTANCE_COUNT_QUERY_URL = CASE_INSTANCE_QUERY_URL + "/count";

	  private CaseInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockCaseInstanceQuery(MockProvider.createMockCaseInstances());
	  }

	  private CaseInstanceQuery setUpMockCaseInstanceQuery(IList<CaseInstance> mockedCaseInstances)
	  {
		CaseInstanceQuery query = mock(typeof(CaseInstanceQuery));

		when(query.list()).thenReturn(mockedCaseInstances);
		when(query.count()).thenReturn((long) mockedCaseInstances.Count);
		when(processEngine.CaseService.createCaseInstanceQuery()).thenReturn(query);

		return query;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryCaseInstanceId = "";

		given().queryParam("caseInstanceId", queryCaseInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQueryAsPost()
	  public virtual void testEmptyQueryAsPost()
	  {
		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["caseInstanceId"] = "";

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("caseInstanceId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "caseInstanceId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		// asc
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		// desc
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).desc();

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
		json["sorting"] = OrderingBuilder.create().orderBy("caseInstanceId").desc().orderBy("caseDefinitionId").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

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

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

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

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceRetrieval()
	  public virtual void testCaseInstanceRetrieval()
	  {
		string queryCaseInstanceId = "aCaseInstanceId";

		Response response = given().queryParam("caseInstanceId", queryCaseInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).caseInstanceId(queryCaseInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> caseInstances = from(content).getList("");

		assertThat(caseInstances).hasSize(1);
		assertThat(caseInstances[0]).NotNull;

		string returnedId = from(content).getString("[0].id");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedBusinessKeyKey = from(content).getString("[0].businessKey");
		string returnedTenantId = from(content).getString("[0].tenantId");
		bool returnedActiveState = from(content).getBoolean("[0].active");
		bool returnedCompletedState = from(content).getBoolean("[0].completed");
		bool returnedTerminatedState = from(content).getBoolean("[0].terminated");

		assertThat(returnedId).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
		assertThat(returnedCaseDefinitionId).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_CASE_DEFINITION_ID);
		assertThat(returnedBusinessKeyKey).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		assertThat(returnedTenantId).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedActiveState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_ACTIVE);
		assertThat(returnedCompletedState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_COMPLETED);
		assertThat(returnedTerminatedState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_TERMINATED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceRetrievalAsPost()
	  public virtual void testCaseInstanceRetrievalAsPost()
	  {
		string queryCaseInstanceId = "aCaseInstanceId";

		IDictionary<string, string> queryParameter = new Dictionary<string, string>();
		queryParameter["caseInstanceId"] = queryCaseInstanceId;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameter).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		// assert query invocation
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		inOrder.verify(mockedQuery).caseInstanceId(queryCaseInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<IDictionary<string, string>> caseInstances = from(content).getList("");

		assertThat(caseInstances).hasSize(1);
		assertThat(caseInstances[0]).NotNull;

		string returnedId = from(content).getString("[0].id");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedBusinessKeyKey = from(content).getString("[0].businessKey");
		string returnedTenantId = from(content).getString("[0].tenantId");
		bool returnedActiveState = from(content).getBoolean("[0].active");
		bool returnedCompletedState = from(content).getBoolean("[0].completed");
		bool returnedTerminatedState = from(content).getBoolean("[0].terminated");

		assertThat(returnedId).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
		assertThat(returnedCaseDefinitionId).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_CASE_DEFINITION_ID);
		assertThat(returnedBusinessKeyKey).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		assertThat(returnedTenantId).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedActiveState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_ACTIVE);
		assertThat(returnedCompletedState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_COMPLETED);
		assertThat(returnedTerminatedState).isEqualTo(MockProvider.EXAMPLE_CASE_INSTANCE_IS_TERMINATED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParameters()
	  public virtual void testMultipleParameters()
	  {
		IDictionary<string, string> queryParameters = new Dictionary<string, string>();

		queryParameters["caseInstanceId"] = "aCaseInstanceId";
		queryParameters["caseDefinitionId"] = "aCaseDefId";
		queryParameters["caseDefinitionKey"] = "aCaseDefKey";
		queryParameters["deploymentId"] = "aDeploymentId";
		queryParameters["businessKey"] = "aBusinessKey";
		queryParameters["superProcessInstance"] = "aSuperProcInstId";
		queryParameters["subProcessInstance"] = "aSubProcInstId";
		queryParameters["superCaseInstance"] = "aSuperCaseInstId";
		queryParameters["subCaseInstance"] = "aSubCaseInstId";
		queryParameters["tenantIdIn"] = "aTenantId";
		queryParameters["active"] = "true";
		queryParameters["completed"] = "true";
		queryParameters["terminated"] = "true";

		given().queryParams(queryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).caseInstanceId(queryParameters["caseInstanceId"]);
		verify(mockedQuery).caseDefinitionId(queryParameters["caseDefinitionId"]);
		verify(mockedQuery).caseDefinitionKey(queryParameters["caseDefinitionKey"]);
		verify(mockedQuery).deploymentId(queryParameters["deploymentId"]);
		verify(mockedQuery).caseInstanceBusinessKey(queryParameters["businessKey"]);
		verify(mockedQuery).superProcessInstanceId(queryParameters["superProcessInstance"]);
		verify(mockedQuery).subProcessInstanceId(queryParameters["subProcessInstance"]);
		verify(mockedQuery).superCaseInstanceId(queryParameters["superCaseInstance"]);
		verify(mockedQuery).subCaseInstanceId(queryParameters["subCaseInstance"]);
		verify(mockedQuery).tenantIdIn(queryParameters["tenantIdIn"]);
		verify(mockedQuery).active();
		verify(mockedQuery).completed();
		verify(mockedQuery).terminated();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleParametersAsPost()
	  public virtual void testMultipleParametersAsPost()
	  {
		string aCaseInstanceId = "aCaseInstanceId";
		string aCaseDefId = "aCaseDefId";
		string aCaseDefKey = "aCaseDefKey";
		string aDeploymentId = "aDeploymentId";
		string aBusinessKey = "aBusinessKey";
		string aSuperProcInstId = "aSuperProcInstId";
		string aSubProcInstId = "aSubProcInstId";
		string aSuperCaseInstId = "aSuperCaseInstId";
		string aSubCaseInstId = "aSubCaseInstId";

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();

		queryParameters["caseInstanceId"] = aCaseInstanceId;
		queryParameters["caseDefinitionId"] = aCaseDefId;
		queryParameters["caseDefinitionKey"] = aCaseDefKey;
		queryParameters["deploymentId"] = aDeploymentId;
		queryParameters["businessKey"] = aBusinessKey;
		queryParameters["superProcessInstance"] = aSuperProcInstId;
		queryParameters["subProcessInstance"] = aSubProcInstId;
		queryParameters["superCaseInstance"] = aSuperCaseInstId;
		queryParameters["subCaseInstance"] = aSubCaseInstId;
		queryParameters["active"] = "true";
		queryParameters["completed"] = "true";
		queryParameters["terminated"] = "true";

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).caseInstanceId(aCaseInstanceId);
		verify(mockedQuery).caseDefinitionId(aCaseDefId);
		verify(mockedQuery).caseDefinitionKey(aCaseDefKey);
		verify(mockedQuery).deploymentId(aDeploymentId);
		verify(mockedQuery).caseInstanceBusinessKey(aBusinessKey);
		verify(mockedQuery).superProcessInstanceId(aSuperProcInstId);
		verify(mockedQuery).subProcessInstanceId(aSubProcInstId);
		verify(mockedQuery).superCaseInstanceId(aSuperCaseInstId);
		verify(mockedQuery).subCaseInstanceId(aSubCaseInstId);
		verify(mockedQuery).active();
		verify(mockedQuery).completed();
		verify(mockedQuery).terminated();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableParameters()
	  public virtual void testVariableParameters()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		// greater then
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		// greater then equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		// lower then
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		// lower then equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableParametersAsPost()
	  public virtual void testVariableParametersAsPost()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["variables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		// greater then
		variableJson["operator"] = "gt";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		// greater then equals
		variableJson["operator"] = "gteq";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		// lower then
		variableJson["operator"] = "lt";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		// lower then equals
		variableJson["operator"] = "lteq";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		// like
		variableJson["operator"] = "like";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueLike(variableName, variableValue);

		// not equals
		variableJson["operator"] = "neq";

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

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

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(CASE_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountAsPost()
	  public virtual void testQueryCountAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(CASE_INSTANCE_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockCaseInstanceQuery(createMockCaseInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> caseInstances = from(content).getList("");
		assertThat(caseInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		mockedQuery = setUpMockCaseInstanceQuery(Arrays.asList(MockProvider.createMockCaseInstance(null)));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> caseInstances = from(content).getList("");
		assertThat(caseInstances).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockCaseInstanceQuery(createMockCaseInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> caseInstances = from(content).getList("");
		assertThat(caseInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdPostParameter()
	  public virtual void testWithoutTenantIdPostParameter()
	  {
		mockedQuery = setUpMockCaseInstanceQuery(Arrays.asList(MockProvider.createMockCaseInstance(null)));

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(CASE_INSTANCE_QUERY_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> caseInstances = from(content).getList("");
		assertThat(caseInstances).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

	  private IList<CaseInstance> createMockCaseInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockCaseInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockCaseInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

	}

}