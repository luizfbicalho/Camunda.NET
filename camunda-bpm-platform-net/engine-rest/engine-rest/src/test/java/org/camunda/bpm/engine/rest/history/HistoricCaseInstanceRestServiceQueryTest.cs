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
namespace org.camunda.bpm.engine.rest.history
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
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using HistoricCaseInstance = org.camunda.bpm.engine.history.HistoricCaseInstance;
	using HistoricCaseInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseInstanceQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricCaseInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_CASE_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/case-instance";
	  protected internal static readonly string HISTORIC_CASE_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_CASE_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricCaseInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricCaseInstanceQuery(MockProvider.createMockHistoricCaseInstances());
	  }

	  protected internal virtual HistoricCaseInstanceQuery setUpMockHistoricCaseInstanceQuery(IList<HistoricCaseInstance> mockedHistoricCaseInstances)
	  {
		HistoricCaseInstanceQuery mockedHistoricCaseInstanceQuery = mock(typeof(HistoricCaseInstanceQuery));
		when(mockedHistoricCaseInstanceQuery.list()).thenReturn(mockedHistoricCaseInstances);
		when(mockedHistoricCaseInstanceQuery.count()).thenReturn((long) mockedHistoricCaseInstances.Count);

		when(processEngine.HistoryService.createHistoricCaseInstanceQuery()).thenReturn(mockedHistoricCaseInstanceQuery);

		return mockedHistoricCaseInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("caseDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("definitionId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "definitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("instanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("instanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("businessKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceBusinessKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("businessKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceBusinessKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("createTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceCreateTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("createTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceCreateTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("closeTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceCloseTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("closeTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceCloseTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceDuration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceDuration();
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
		json["sorting"] = OrderingBuilder.create().orderBy("businessKey").desc().orderBy("closeTime").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByCaseInstanceBusinessKey();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByCaseInstanceCloseTime();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_CASE_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().body("count", equalTo(1)).when().post(HISTORIC_CASE_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricCaseQuery()
	  public virtual void testSimpleHistoricCaseQuery()
	  {
		string caseInstanceId = MockProvider.EXAMPLE_CASE_INSTANCE_ID;

		Response response = given().queryParam("caseInstanceId", caseInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).caseInstanceId(caseInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedCaseInstanceId = from(content).getString("[0].id");
		string returnedCaseInstanceBusinessKey = from(content).getString("[0].businessKey");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedCaseDefinitionKey = from(content).getString("[0].caseDefinitionKey");
		string returnedCaseDefinitionName = from(content).getString("[0].caseDefinitionName");
		string returnedCreateTime = from(content).getString("[0].createTime");
		string returnedCloseTime = from(content).getString("[0].closeTime");
		long returnedDurationInMillis = from(content).getLong("[0].durationInMillis");
		string returnedCreateUserId = from(content).getString("[0].createUserId");
		string returnedSuperCaseInstanceId = from(content).getString("[0].superCaseInstanceId");
		string returnedSuperProcessInstanceId = from(content).getString("[0].superProcessInstanceId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		bool active = from(content).getBoolean("[0].active");
		bool completed = from(content).getBoolean("[0].completed");
		bool terminated = from(content).getBoolean("[0].terminated");
		bool closed = from(content).getBoolean("[0].closed");

		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY, returnedCaseInstanceBusinessKey);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_KEY, returnedCaseDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_NAME, returnedCaseDefinitionName);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_TIME, returnedCreateTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSE_TIME, returnedCloseTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_DURATION_MILLIS, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATE_USER_ID, returnedCreateUserId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_CASE_INSTANCE_ID, returnedSuperCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_PROCESS_INSTANCE_ID, returnedSuperProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_IS_ACTIVE, active);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_IS_COMPLETED, completed);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_IS_TERMINATED, terminated);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_IS_CLOSED, closed);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingCases()
	  public virtual void testAdditionalParametersExcludingCases()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingCasesAsPost()
	  public virtual void testAdditionalParametersExcludingCasesAsPost()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterCreateTimeQuery()
	  public virtual void testHistoricBeforeAndAfterCreateTimeQuery()
	  {
		given().queryParam("createdBefore", MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_BEFORE).queryParam("createdAfter", MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCreateParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterCreateTimeQueryAsPost()
	  public virtual void testHistoricBeforeAndAfterCreateTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteCreateDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCreateParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterCreateTimeAsStringQueryAsPost()
	  public virtual void testHistoricBeforeAndAfterCreateTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteCreateDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyStringCreateParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeCloseTimeQuery()
	  public virtual void testHistoricAfterAndBeforeCloseTimeQuery()
	  {
		given().queryParam("closedAfter", MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_AFTER).queryParam("closedBefore", MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyClosedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeCloseTimeQueryAsPost()
	  public virtual void testHistoricAfterAndBeforeCloseTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteClosedDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyClosedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeCloseTimeAsStringQueryAsPost()
	  public virtual void testHistoricAfterAndBeforeCloseTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteClosedDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyStringClosedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActiveClosed()
	  public virtual void testCaseActiveClosed()
	  {
		given().queryParam("active", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).active();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryActiveAsPost()
	  public virtual void testCaseQueryActiveAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["active"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).active();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryCompleted()
	  public virtual void testCaseQueryCompleted()
	  {
		given().queryParam("completed", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).completed();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryCompletedAsPost()
	  public virtual void testCaseQueryCompletedAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["completed"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).completed();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryTerminated()
	  public virtual void testCaseQueryTerminated()
	  {
		given().queryParam("terminated", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).terminated();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryTerminatedAsPost()
	  public virtual void testCaseQueryTerminatedAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["terminated"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).terminated();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryClosed()
	  public virtual void testCaseQueryClosed()
	  {
		given().queryParam("closed", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).closed();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryClosedAsPost()
	  public virtual void testCaseQueryClosedAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["closed"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).closed();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryNotClosed()
	  public virtual void testCaseQueryNotClosed()
	  {
		IList<HistoricCaseInstance> mockedHistoricCaseInstances = MockProvider.createMockRunningHistoricCaseInstances();
		HistoricCaseInstanceQuery mockedHistoricCaseInstanceQuery = mock(typeof(HistoricCaseInstanceQuery));
		when(mockedHistoricCaseInstanceQuery.list()).thenReturn(mockedHistoricCaseInstances);
		when(processEngine.HistoryService.createHistoricCaseInstanceQuery()).thenReturn(mockedHistoricCaseInstanceQuery);

		Response response = given().queryParam("notClosed", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedHistoricCaseInstanceQuery);
		inOrder.verify(mockedHistoricCaseInstanceQuery).notClosed();
		inOrder.verify(mockedHistoricCaseInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedCaseInstanceId = from(content).getString("[0].id");
		string returnedCloseTime = from(content).getString("[0].closeTime");

		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(null, returnedCloseTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseQueryNotClosedAsPost()
	  public virtual void testCaseQueryNotClosedAsPost()
	  {
		IList<HistoricCaseInstance> mockedHistoricCaseInstances = MockProvider.createMockRunningHistoricCaseInstances();
		HistoricCaseInstanceQuery mockedHistoricCaseInstanceQuery = mock(typeof(HistoricCaseInstanceQuery));
		when(mockedHistoricCaseInstanceQuery.list()).thenReturn(mockedHistoricCaseInstances);
		when(processEngine.HistoryService.createHistoricCaseInstanceQuery()).thenReturn(mockedHistoricCaseInstanceQuery);

		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["notClosed"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedHistoricCaseInstanceQuery);
		inOrder.verify(mockedHistoricCaseInstanceQuery).notClosed();
		inOrder.verify(mockedHistoricCaseInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedCaseInstanceId = from(content).getString("[0].id");
		string returnedCloseTime = from(content).getString("[0].closeTime");

		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(null, returnedCloseTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseInstanceIds()
	  public virtual void testQueryByCaseInstanceIds()
	  {
		given().queryParam("caseInstanceIds", "firstCaseInstanceId,secondCaseInstanceId").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCaseInstanceIdSetInvocation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseInstanceIdsAsPost()
	  public virtual void testQueryByCaseInstanceIdsAsPost()
	  {
		IDictionary<string, ISet<string>> parameters = CompleteCaseInstanceIdSetQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCaseInstanceIdSetInvocation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionKeyNotIn()
	  public virtual void testQueryByCaseDefinitionKeyNotIn()
	  {
		given().queryParam("caseDefinitionKeyNotIn", "firstCaseInstanceKey,secondCaseInstanceKey").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCaseDefinitionKeyNotInListInvocation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionKeyNotInAsPost()
	  public virtual void testQueryByCaseDefinitionKeyNotInAsPost()
	  {
		IDictionary<string, IList<string>> parameters = CompleteCaseDefinitionKeyNotInListQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verifyCaseDefinitionKeyNotInListInvocation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableParameters()
	  public virtual void testVariableParameters()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		// greater then
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		// greater then equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		// lower then
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		// lower then equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

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

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricCaseInstanceQuery(createMockHistoricCaseInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicCaseInstances = from(content).getList("");
		assertThat(historicCaseInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockHistoricCaseInstanceQuery(createMockHistoricCaseInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicCaseInstances = from(content).getList("");
		assertThat(historicCaseInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		mockedQuery = setUpMockHistoricCaseInstanceQuery(Arrays.asList(MockProvider.createMockHistoricCaseInstance(null)));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdPostParameter()
	  public virtual void testWithoutTenantIdPostParameter()
	  {
		mockedQuery = setUpMockHistoricCaseInstanceQuery(Arrays.asList(MockProvider.createMockHistoricCaseInstance(null)));

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

	  private IList<HistoricCaseInstance> createMockHistoricCaseInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricCaseInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricCaseInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActivityIdListParameter()
	  public virtual void testCaseActivityIdListParameter()
	  {

		Response response = given().queryParam("caseActivityIdIn", MockProvider.EXAMPLE_CASE_ACTIVITY_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityIdIn(MockProvider.EXAMPLE_CASE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicCaseInstances = from(content).getList("");
		assertThat(historicCaseInstances).hasSize(1);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActivityIdListPostParameter()
	  public virtual void testCaseActivityIdListPostParameter()
	  {

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["caseActivityIdIn"] = MockProvider.EXAMPLE_CASE_ACTIVITY_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_CASE_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityIdIn(MockProvider.EXAMPLE_CASE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_CASE_ACTIVITY_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicCaseInstances = from(content).getList("");
		assertThat(historicCaseInstances).hasSize(1);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_CASE_INSTANCE_RESOURCE_URL);
	  }

	  protected internal virtual IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["caseInstanceId"] = MockProvider.EXAMPLE_CASE_INSTANCE_ID;
			parameters["caseInstanceBusinessKey"] = MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY;
			parameters["caseInstanceBusinessKeyLike"] = MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY_LIKE;
			parameters["caseDefinitionId"] = MockProvider.EXAMPLE_CASE_DEFINITION_ID;
			parameters["caseDefinitionKey"] = MockProvider.EXAMPLE_CASE_DEFINITION_KEY;
			parameters["caseDefinitionName"] = MockProvider.EXAMPLE_CASE_DEFINITION_NAME;
			parameters["caseDefinitionNameLike"] = MockProvider.EXAMPLE_CASE_DEFINITION_NAME_LIKE;
			parameters["createdBy"] = "createdBySomeone";
			parameters["superCaseInstanceId"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_CASE_INSTANCE_ID;
			parameters["subCaseInstanceId"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUB_CASE_INSTANCE_ID;
			parameters["superProcessInstanceId"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUPER_PROCESS_INSTANCE_ID;
			parameters["subProcessInstanceId"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_SUB_PROCESS_INSTANCE_ID;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);
		verify(mockedQuery).caseInstanceBusinessKey(stringQueryParameters["caseInstanceBusinessKey"]);
		verify(mockedQuery).caseInstanceBusinessKeyLike(stringQueryParameters["caseInstanceBusinessKeyLike"]);
		verify(mockedQuery).caseDefinitionId(stringQueryParameters["caseDefinitionId"]);
		verify(mockedQuery).caseDefinitionKey(stringQueryParameters["caseDefinitionKey"]);
		verify(mockedQuery).caseDefinitionName(stringQueryParameters["caseDefinitionName"]);
		verify(mockedQuery).caseDefinitionNameLike(stringQueryParameters["caseDefinitionNameLike"]);
		verify(mockedQuery).createdBy(stringQueryParameters["createdBy"]);
		verify(mockedQuery).superCaseInstanceId(stringQueryParameters["superCaseInstanceId"]);
		verify(mockedQuery).subCaseInstanceId(stringQueryParameters["subCaseInstanceId"]);
		verify(mockedQuery).superProcessInstanceId(stringQueryParameters["superProcessInstanceId"]);
		verify(mockedQuery).subProcessInstanceId(stringQueryParameters["subProcessInstanceId"]);
		verify(mockedQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, DateTime> CompleteCreateDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["createdAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_AFTER);
			parameters["createdBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_BEFORE);
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyCreateParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> createDateParameters = CompleteCreateDateQueryParameters;

		verify(mockedQuery).createdBefore(createDateParameters["createdBefore"]);
		verify(mockedQuery).createdAfter(createDateParameters["createdAfter"]);

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, string> CompleteCreateDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["createdAfter"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_AFTER;
			parameters["createdBefore"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CREATED_BEFORE;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringCreateParameterQueryInvocations()
	  {
		IDictionary<string, string> createDateParameters = CompleteCreateDateAsStringQueryParameters;

		verify(mockedQuery).createdBefore(DateTimeUtil.parseDate(createDateParameters["createdBefore"]));
		verify(mockedQuery).createdAfter(DateTimeUtil.parseDate(createDateParameters["createdAfter"]));

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, DateTime> CompleteClosedDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["closedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_AFTER);
			parameters["closedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_BEFORE);
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyClosedParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> closedDateParameters = CompleteClosedDateQueryParameters;

		verify(mockedQuery).closedAfter(closedDateParameters["closedAfter"]);
		verify(mockedQuery).closedBefore(closedDateParameters["closedBefore"]);

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, string> CompleteClosedDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["closedAfter"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_AFTER;
			parameters["closedBefore"] = MockProvider.EXAMPLE_HISTORIC_CASE_INSTANCE_CLOSED_BEFORE;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringClosedParameterQueryInvocations()
	  {
		IDictionary<string, string> closedDateParameters = CompleteClosedDateAsStringQueryParameters;

		verify(mockedQuery).closedAfter(DateTimeUtil.parseDate(closedDateParameters["closedAfter"]));
		verify(mockedQuery).closedBefore(DateTimeUtil.parseDate(closedDateParameters["closedBefore"]));

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, ISet<string>> CompleteCaseInstanceIdSetQueryParameters
	  {
		  get
		  {
			IDictionary<string, ISet<string>> parameters = new Dictionary<string, ISet<string>>();
    
			ISet<string> caseInstanceIds = new HashSet<string>();
			caseInstanceIds.Add("firstCaseInstanceId");
			caseInstanceIds.Add("secondCaseInstanceId");
    
			parameters["caseInstanceIds"] = caseInstanceIds;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyCaseInstanceIdSetInvocation()
	  {
		IDictionary<string, ISet<string>> parameters = CompleteCaseInstanceIdSetQueryParameters;

		verify(mockedQuery).caseInstanceIds(parameters["caseInstanceIds"]);
		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, IList<string>> CompleteCaseDefinitionKeyNotInListQueryParameters
	  {
		  get
		  {
			IDictionary<string, IList<string>> parameters = new Dictionary<string, IList<string>>();
    
			IList<string> caseInstanceIds = new List<string>();
			caseInstanceIds.Add("firstCaseInstanceKey");
			caseInstanceIds.Add("secondCaseInstanceKey");
    
			parameters["caseDefinitionKeyNotIn"] = caseInstanceIds;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyCaseDefinitionKeyNotInListInvocation()
	  {
		IDictionary<string, IList<string>> parameters = CompleteCaseDefinitionKeyNotInListQueryParameters;

		verify(mockedQuery).caseDefinitionKeyNotIn(parameters["caseDefinitionKeyNotIn"]);
		verify(mockedQuery).list();
	  }

	}

}