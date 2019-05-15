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
	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
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
//	import static org.mockito.Mockito.doThrow;
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


	public class HistoricProcessInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

	  protected internal const string QUERY_PARAM_EXECUTED_JOB_BEFORE = "executedJobBefore";
	  protected internal const string QUERY_PARAM_EXECUTED_JOB_AFTER = "executedJobAfter";
	  protected internal const string QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE = "executedActivityBefore";
	  protected internal const string QUERY_PARAM_EXECUTED_ACTIVITY_AFTER = "executedActivityAfter";
	  protected internal const string QUERY_PARAM_EXECUTED_ACTIVITY_IDS = "executedActivityIdIn";
	  protected internal const string QUERY_PARAM_ACTIVE_ACTIVITY_IDS = "activeActivityIdIn";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_PROCESS_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/process-instance";
	  protected internal static readonly string HISTORIC_PROCESS_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_PROCESS_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricProcessInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricProcessInstanceQuery(MockProvider.createMockHistoricProcessInstances());
	  }

	  private HistoricProcessInstanceQuery setUpMockHistoricProcessInstanceQuery(IList<HistoricProcessInstance> mockedHistoricProcessInstances)
	  {
		HistoricProcessInstanceQuery mockedhistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(mockedhistoricProcessInstanceQuery.list()).thenReturn(mockedHistoricProcessInstances);
		when(mockedhistoricProcessInstanceQuery.count()).thenReturn((long) mockedHistoricProcessInstances.Count);

		when(processEngine.HistoryService.createHistoricProcessInstanceQuery()).thenReturn(mockedhistoricProcessInstanceQuery);

		return mockedhistoricProcessInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);
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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "definitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);
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
		executeAndVerifySorting("instanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("definitionVersion", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionVersion();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("businessKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceBusinessKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("businessKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceBusinessKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("startTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceStartTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("startTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceStartTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceEndTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceEndTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceDuration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceDuration();
		inOrder.verify(mockedQuery).desc();

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
		json["sorting"] = OrderingBuilder.create().orderBy("instanceId").desc().orderBy("startTime").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByProcessInstanceStartTime();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_PROCESS_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().body("count", equalTo(1)).when().post(HISTORIC_PROCESS_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricProcessQuery()
	  public virtual void testSimpleHistoricProcessQuery()
	  {
		string processInstanceId = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		Response response = given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processInstanceId(processInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one process instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned process instance should not be null.", instances[0]);

		string returnedProcessInstanceId = from(content).getString("[0].id");
		string returnedProcessInstanceBusinessKey = from(content).getString("[0].businessKey");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedProcessDefinitionName = from(content).getString("[0].processDefinitionName");
		int returnedProcessDefinitionVersion = from(content).getInt("[0].processDefinitionVersion");
		string returnedStartTime = from(content).getString("[0].startTime");
		string returnedEndTime = from(content).getString("[0].endTime");
		string returnedRemovalTime = from(content).getString("[0].removalTime");
		long returnedDurationInMillis = from(content).getLong("[0].durationInMillis");
		string returnedStartUserId = from(content).getString("[0].startUserId");
		string returnedStartActivityId = from(content).getString("[0].startActivityId");
		string returnedDeleteReason = from(content).getString("[0].deleteReason");
		string returnedRootProcessInstanceId = from(content).getString("[0].rootProcessInstanceId");
		string returnedSuperProcessInstanceId = from(content).getString("[0].superProcessInstanceId");
		string returnedSuperCaseInstanceId = from(content).getString("[0].superCaseInstanceId");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		string returnedState = from(content).getString("[0].state");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY, returnedProcessInstanceBusinessKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME, returnedProcessDefinitionName);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION, returnedProcessDefinitionVersion);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_TIME, returnedStartTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_END_TIME, returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_REMOVAL_TIME, returnedRemovalTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DURATION_MILLIS, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_USER_ID, returnedStartUserId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_START_ACTIVITY_ID, returnedStartActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_DELETE_REASON, returnedDeleteReason);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_ROOT_PROCESS_INSTANCE_ID, returnedRootProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_PROCESS_INSTANCE_ID, returnedSuperProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_CASE_INSTANCE_ID, returnedSuperCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STATE, returnedState);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingProcesses()
	  public virtual void testAdditionalParametersExcludingProcesses()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingProcessesAsPost()
	  public virtual void testAdditionalParametersExcludingProcessesAsPost()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;
			parameters["processInstanceBusinessKey"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY;
			parameters["processInstanceBusinessKeyLike"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY_LIKE;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
			parameters["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
			parameters["processDefinitionName"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME;
			parameters["processDefinitionNameLike"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME_LIKE;
			parameters["startedBy"] = "startedBySomeone";
			parameters["superProcessInstanceId"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_PROCESS_INSTANCE_ID;
			parameters["subProcessInstanceId"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUB_PROCESS_INSTANCE_ID;
			parameters["superCaseInstanceId"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUPER_CASE_INSTANCE_ID;
			parameters["subCaseInstanceId"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_SUB_CASE_INSTANCE_ID;
			parameters["caseInstanceId"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_CASE_INSTANCE_ID;
			parameters["state"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STATE;
			parameters["incidentType"] = MockProvider.EXAMPLE_INCIDENT_TYPE;
    
			return parameters;
		  }
	  }

	  private void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockedQuery).processInstanceBusinessKey(stringQueryParameters["processInstanceBusinessKey"]);
		verify(mockedQuery).processInstanceBusinessKeyLike(stringQueryParameters["processInstanceBusinessKeyLike"]);
		verify(mockedQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionKey(stringQueryParameters["processDefinitionKey"]);
		verify(mockedQuery).processDefinitionName(stringQueryParameters["processDefinitionName"]);
		verify(mockedQuery).processDefinitionNameLike(stringQueryParameters["processDefinitionNameLike"]);
		verify(mockedQuery).startedBy(stringQueryParameters["startedBy"]);
		verify(mockedQuery).superProcessInstanceId(stringQueryParameters["superProcessInstanceId"]);
		verify(mockedQuery).subProcessInstanceId(stringQueryParameters["subProcessInstanceId"]);
		verify(mockedQuery).superCaseInstanceId(stringQueryParameters["superCaseInstanceId"]);
		verify(mockedQuery).subCaseInstanceId(stringQueryParameters["subCaseInstanceId"]);
		verify(mockedQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);
		verify(mockedQuery).incidentType(stringQueryParameters["incidentType"]);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeQuery()
	  public virtual void testHistoricBeforeAndAfterStartTimeQuery()
	  {
		given().queryParam("startedBefore", MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE).queryParam("startedAfter", MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStartParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeQueryAsPost()
	  public virtual void testHistoricBeforeAndAfterStartTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteStartDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStartParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeAsStringQueryAsPost()
	  public virtual void testHistoricBeforeAndAfterStartTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteStartDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringStartParameterQueryInvocations();
	  }

	  private IDictionary<string, DateTime> CompleteStartDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["startedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER);
			parameters["startedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE);
    
			return parameters;
		  }
	  }

	  private IDictionary<string, string> CompleteStartDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["startedAfter"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER;
			parameters["startedBefore"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE;
    
			return parameters;
		  }
	  }

	  private void verifyStartParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> startDateParameters = CompleteStartDateQueryParameters;

		verify(mockedQuery).startedBefore(startDateParameters["startedBefore"]);
		verify(mockedQuery).startedAfter(startDateParameters["startedAfter"]);

		verify(mockedQuery).list();
	  }

	  private void verifyStringStartParameterQueryInvocations()
	  {
		IDictionary<string, string> startDateParameters = CompleteStartDateAsStringQueryParameters;

		verify(mockedQuery).startedBefore(DateTimeUtil.parseDate(startDateParameters["startedBefore"]));
		verify(mockedQuery).startedAfter(DateTimeUtil.parseDate(startDateParameters["startedAfter"]));

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeFinishTimeQuery()
	  public virtual void testHistoricAfterAndBeforeFinishTimeQuery()
	  {
		given().queryParam("finishedAfter", MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_AFTER).queryParam("finishedBefore", MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyFinishedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeFinishTimeQueryAsPost()
	  public virtual void testHistoricAfterAndBeforeFinishTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteFinishedDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyFinishedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeFinishTimeAsStringQueryAsPost()
	  public virtual void testHistoricAfterAndBeforeFinishTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteFinishedDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringFinishedParameterQueryInvocations();
	  }

	  private IDictionary<string, DateTime> CompleteFinishedDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["finishedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_AFTER);
			parameters["finishedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_BEFORE);
    
			return parameters;
		  }
	  }

	  private IDictionary<string, string> CompleteFinishedDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["finishedAfter"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_AFTER;
			parameters["finishedBefore"] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_FINISHED_BEFORE;
    
			return parameters;
		  }
	  }

	  private void verifyFinishedParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> finishedDateParameters = CompleteFinishedDateQueryParameters;

		verify(mockedQuery).finishedAfter(finishedDateParameters["finishedAfter"]);
		verify(mockedQuery).finishedBefore(finishedDateParameters["finishedBefore"]);

		verify(mockedQuery).list();
	  }

	  private void verifyStringFinishedParameterQueryInvocations()
	  {
		IDictionary<string, string> finishedDateParameters = CompleteFinishedDateAsStringQueryParameters;

		verify(mockedQuery).finishedAfter(DateTimeUtil.parseDate(finishedDateParameters["finishedAfter"]));
		verify(mockedQuery).finishedBefore(DateTimeUtil.parseDate(finishedDateParameters["finishedBefore"]));

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessQueryFinished()
	  public virtual void testProcessQueryFinished()
	  {
		given().queryParam("finished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).finished();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessQueryFinishedAsPost()
	  public virtual void testProcessQueryFinishedAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["finished"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).finished();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessQueryUnfinished()
	  public virtual void testProcessQueryUnfinished()
	  {
		IList<HistoricProcessInstance> mockedHistoricProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		HistoricProcessInstanceQuery mockedhistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(mockedhistoricProcessInstanceQuery.list()).thenReturn(mockedHistoricProcessInstances);
		when(processEngine.HistoryService.createHistoricProcessInstanceQuery()).thenReturn(mockedhistoricProcessInstanceQuery);

		Response response = given().queryParam("unfinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedhistoricProcessInstanceQuery);
		inOrder.verify(mockedhistoricProcessInstanceQuery).unfinished();
		inOrder.verify(mockedhistoricProcessInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one process instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned process instance should not be null.", instances[0]);

		string returnedProcessInstanceId = from(content).getString("[0].id");
		string returnedEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(null, returnedEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessQueryUnfinishedAsPost()
	  public virtual void testProcessQueryUnfinishedAsPost()
	  {
		IList<HistoricProcessInstance> mockedHistoricProcessInstances = MockProvider.createMockRunningHistoricProcessInstances();
		HistoricProcessInstanceQuery mockedhistoricProcessInstanceQuery = mock(typeof(HistoricProcessInstanceQuery));
		when(mockedhistoricProcessInstanceQuery.list()).thenReturn(mockedHistoricProcessInstances);
		when(processEngine.HistoryService.createHistoricProcessInstanceQuery()).thenReturn(mockedhistoricProcessInstanceQuery);

		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["unfinished"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedhistoricProcessInstanceQuery);
		inOrder.verify(mockedhistoricProcessInstanceQuery).unfinished();
		inOrder.verify(mockedhistoricProcessInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one process instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned process instance should not be null.", instances[0]);

		string returnedProcessInstanceId = from(content).getString("[0].id");
		string returnedEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(null, returnedEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidents()
	  public virtual void testQueryWithIncidents()
	  {
		given().queryParam("withIncidents", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).withIncidents();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidentsAsPost()
	  public virtual void testQueryWithIncidentsAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["withIncidents"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).withIncidents();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidentStatusOpen()
	  public virtual void testQueryWithIncidentStatusOpen()
	  {
		given().queryParam("incidentStatus", "open").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentStatus("open");
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidentStatusOpenAsPost()
	  public virtual void testQueryWithIncidentStatusOpenAsPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentStatus"] = "open";

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentStatus("open");
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountIncidentStatusOpenForPost()
	  public virtual void testQueryCountIncidentStatusOpenForPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentStatus"] = "open";
		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().body("count", equalTo(1)).when().post(HISTORIC_PROCESS_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
		verify(mockedQuery).incidentStatus("open");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidentStatusResolved()
	  public virtual void testQueryWithIncidentStatusResolved()
	  {
		given().queryParam("incidentStatus", "resolved").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentStatus("resolved");
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithIncidentStatusResolvedAsPost()
	  public virtual void testQueryWithIncidentStatusResolvedAsPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentStatus"] = "resolved";

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentStatus("resolved");
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountIncidentStatusResolvedForPost()
	  public virtual void testQueryCountIncidentStatusResolvedForPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentStatus"] = "resolved";
		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().body("count", equalTo(1)).when().post(HISTORIC_PROCESS_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
		verify(mockedQuery).incidentStatus("resolved");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentType()
	  public virtual void testQueryIncidentType()
	  {
		given().queryParam("incidentType", MockProvider.EXAMPLE_INCIDENT_TYPE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentType(MockProvider.EXAMPLE_INCIDENT_TYPE);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentTypeAsPost()
	  public virtual void testQueryIncidentTypeAsPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentType"] = MockProvider.EXAMPLE_INCIDENT_TYPE;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentType(MockProvider.EXAMPLE_INCIDENT_TYPE);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentMessage()
	  public virtual void testQueryIncidentMessage()
	  {
		given().queryParam("incidentMessage", MockProvider.EXAMPLE_INCIDENT_MESSAGE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentMessage(MockProvider.EXAMPLE_INCIDENT_MESSAGE);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentMessageAsPost()
	  public virtual void testQueryIncidentMessageAsPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentMessage"] = MockProvider.EXAMPLE_INCIDENT_MESSAGE;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentMessage(MockProvider.EXAMPLE_INCIDENT_MESSAGE);
		inOrder.verify(mockedQuery).list();
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentMessageLike()
	  public virtual void testQueryIncidentMessageLike()
	  {
		given().queryParam("incidentMessageLike", MockProvider.EXAMPLE_INCIDENT_MESSAGE_LIKE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentMessageLike(MockProvider.EXAMPLE_INCIDENT_MESSAGE_LIKE);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryIncidentMessageLikeAsPost()
	  public virtual void testQueryIncidentMessageLikeAsPost()
	  {
		IDictionary<string, string> body = new Dictionary<string, string>();
		body["incidentMessageLike"] = MockProvider.EXAMPLE_INCIDENT_MESSAGE_LIKE;

		given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).incidentMessageLike(MockProvider.EXAMPLE_INCIDENT_MESSAGE_LIKE);
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIds()
	  public virtual void testQueryByProcessInstanceIds()
	  {
		given().queryParam("processInstanceIds", "firstProcessInstanceId,secondProcessInstanceId").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyProcessInstanceIdSetInvovation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIdsAsPost()
	  public virtual void testQueryByProcessInstanceIdsAsPost()
	  {
		IDictionary<string, ISet<string>> parameters = CompleteProcessInstanceIdSetQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyProcessInstanceIdSetInvovation();
	  }

	  private IDictionary<string, ISet<string>> CompleteProcessInstanceIdSetQueryParameters
	  {
		  get
		  {
			IDictionary<string, ISet<string>> parameters = new Dictionary<string, ISet<string>>();
    
			ISet<string> processInstanceIds = new HashSet<string>();
			processInstanceIds.Add("firstProcessInstanceId");
			processInstanceIds.Add("secondProcessInstanceId");
    
			parameters["processInstanceIds"] = processInstanceIds;
    
			return parameters;
		  }
	  }

	  private void verifyProcessInstanceIdSetInvovation()
	  {
		IDictionary<string, ISet<string>> parameters = CompleteProcessInstanceIdSetQueryParameters;

		verify(mockedQuery).processInstanceIds(parameters["processInstanceIds"]);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKeyNotIn()
	  public virtual void testQueryByProcessDefinitionKeyNotIn()
	  {
		given().queryParam("processDefinitionKeyNotIn", "firstProcessInstanceKey,secondProcessInstanceKey").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyProcessDefinitionKeyNotInListInvovation();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKeyNotInAsPost()
	  public virtual void testQueryByProcessDefinitionKeyNotInAsPost()
	  {
		IDictionary<string, IList<string>> parameters = CompleteProcessDefinitionKeyNotInListQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyProcessDefinitionKeyNotInListInvovation();
	  }

	  private IDictionary<string, IList<string>> CompleteProcessDefinitionKeyNotInListQueryParameters
	  {
		  get
		  {
			IDictionary<string, IList<string>> parameters = new Dictionary<string, IList<string>>();
    
			IList<string> processInstanceIds = new List<string>();
			processInstanceIds.Add("firstProcessInstanceKey");
			processInstanceIds.Add("secondProcessInstanceKey");
    
			parameters["processDefinitionKeyNotIn"] = processInstanceIds;
    
			return parameters;
		  }
	  }

	  private void verifyProcessDefinitionKeyNotInListInvovation()
	  {
		IDictionary<string, IList<string>> parameters = CompleteProcessDefinitionKeyNotInListQueryParameters;

		verify(mockedQuery).processDefinitionKeyNotIn(parameters["processDefinitionKeyNotIn"]);
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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);

		// greater then
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueGreaterThan(variableName, variableValue);

		// greater then equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueGreaterThanOrEqual(variableName, variableValue);

		// lower then
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLessThan(variableName, variableValue);

		// lower then equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLessThanOrEqual(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

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

		given().queryParam("variables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

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

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).variableValueEquals(variableName, variableValue);
		verify(mockedQuery).variableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricProcessInstanceQuery(createMockHistoricProcessInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

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
		mockedQuery = setUpMockHistoricProcessInstanceQuery(createMockHistoricProcessInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

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
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		mockedQuery = setUpMockHistoricProcessInstanceQuery(Collections.singletonList(MockProvider.createMockHistoricProcessInstance(null)));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

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
		mockedQuery = setUpMockHistoricProcessInstanceQuery(Collections.singletonList(MockProvider.createMockHistoricProcessInstance(null)));

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withoutTenantId();
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(1);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId1).isEqualTo(null);
	  }

	  private IList<HistoricProcessInstance> createMockHistoricProcessInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricProcessInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricProcessInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedActivityBeforeAndAfterTimeQuery()
	  public virtual void testExecutedActivityBeforeAndAfterTimeQuery()
	  {
		given().queryParam(QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE, MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE).queryParam(QUERY_PARAM_EXECUTED_ACTIVITY_AFTER, MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyExecutedActivityParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedActivityBeforeAndAfterTimeQueryAsPost()
	  public virtual void testExecutedActivityBeforeAndAfterTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteExecutedActivityDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyExecutedActivityParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedActivityBeforeAndAfterTimeAsStringQueryAsPost()
	  public virtual void testExecutedActivityBeforeAndAfterTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteExecutedActivityDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringExecutedActivityParameterQueryInvocations();
	  }


	  private void verifyExecutedActivityParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> startDateParameters = CompleteExecutedActivityDateQueryParameters;

		verify(mockedQuery).executedActivityBefore(startDateParameters[QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE]);
		verify(mockedQuery).executedActivityAfter(startDateParameters[QUERY_PARAM_EXECUTED_ACTIVITY_AFTER]);

		verify(mockedQuery).list();
	  }

	  private void verifyStringExecutedActivityParameterQueryInvocations()
	  {
		IDictionary<string, string> startDateParameters = CompleteExecutedActivityDateAsStringQueryParameters;

		verify(mockedQuery).executedActivityBefore(DateTimeUtil.parseDate(startDateParameters[QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE]));
		verify(mockedQuery).executedActivityAfter(DateTimeUtil.parseDate(startDateParameters[QUERY_PARAM_EXECUTED_ACTIVITY_AFTER]));

		verify(mockedQuery).list();
	  }

	  private IDictionary<string, DateTime> CompleteExecutedActivityDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters[QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE);
			parameters[QUERY_PARAM_EXECUTED_ACTIVITY_AFTER] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER);
    
			return parameters;
		  }
	  }

	  private IDictionary<string, string> CompleteExecutedActivityDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters[QUERY_PARAM_EXECUTED_ACTIVITY_AFTER] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER;
			parameters[QUERY_PARAM_EXECUTED_ACTIVITY_BEFORE] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE;
    
			return parameters;
		  }
	  }

	  // ===================================================================================================================

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedJobBeforeAndAfterTimeQuery()
	  public virtual void testExecutedJobBeforeAndAfterTimeQuery()
	  {
		given().queryParam(QUERY_PARAM_EXECUTED_JOB_BEFORE, MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE).queryParam(QUERY_PARAM_EXECUTED_JOB_AFTER, MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyExecutedJobParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedJobBeforeAndAfterTimeQueryAsPost()
	  public virtual void testExecutedJobBeforeAndAfterTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteExecutedJobDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyExecutedJobParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedJobBeforeAndAfterTimeAsStringQueryAsPost()
	  public virtual void testExecutedJobBeforeAndAfterTimeAsStringQueryAsPost()
	  {
		IDictionary<string, string> parameters = CompleteExecutedJobDateAsStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verifyStringExecutedJobParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedActivityIdIn()
	  public virtual void testExecutedActivityIdIn()
	  {

		given().queryParameter(QUERY_PARAM_EXECUTED_ACTIVITY_IDS, "1,2").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).executedActivityIdIn("1", "2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecutedActivityIdInAsPost()
	  public virtual void testExecutedActivityIdInAsPost()
	  {
		IDictionary<string, IList<string>> parameters = new Dictionary<string, IList<string>>();
		parameters[QUERY_PARAM_EXECUTED_ACTIVITY_IDS] = Arrays.asList("1", "2");

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).executedActivityIdIn("1", "2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActiveActivityIdIn()
	  public virtual void testActiveActivityIdIn()
	  {

		given().queryParameter(QUERY_PARAM_ACTIVE_ACTIVITY_IDS, "1,2").then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activeActivityIdIn("1", "2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActiveActivityIdInAsPost()
	  public virtual void testActiveActivityIdInAsPost()
	  {
		IDictionary<string, IList<string>> parameters = new Dictionary<string, IList<string>>();
		parameters[QUERY_PARAM_ACTIVE_ACTIVITY_IDS] = Arrays.asList("1", "2");

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activeActivityIdIn("1", "2");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithRootIncidents()
	  public virtual void testQueryWithRootIncidents()
	  {
		given().queryParam("withRootIncidents", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).withRootIncidents();
		inOrder.verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithRootIncidentsAsPost()
	  public virtual void testQueryWithRootIncidentsAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["withRootIncidents"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withRootIncidents();
	  }

	  private void verifyExecutedJobParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> startDateParameters = CompleteExecutedJobDateQueryParameters;

		verify(mockedQuery).executedJobBefore(startDateParameters[QUERY_PARAM_EXECUTED_JOB_BEFORE]);
		verify(mockedQuery).executedJobAfter(startDateParameters[QUERY_PARAM_EXECUTED_JOB_AFTER]);

		verify(mockedQuery).list();
	  }

	  private void verifyStringExecutedJobParameterQueryInvocations()
	  {
		IDictionary<string, string> startDateParameters = CompleteExecutedJobDateAsStringQueryParameters;

		verify(mockedQuery).executedJobBefore(DateTimeUtil.parseDate(startDateParameters[QUERY_PARAM_EXECUTED_JOB_BEFORE]));
		verify(mockedQuery).executedJobAfter(DateTimeUtil.parseDate(startDateParameters[QUERY_PARAM_EXECUTED_JOB_AFTER]));

		verify(mockedQuery).list();
	  }

	  private IDictionary<string, DateTime> CompleteExecutedJobDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters[QUERY_PARAM_EXECUTED_JOB_BEFORE] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE);
			parameters[QUERY_PARAM_EXECUTED_JOB_AFTER] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER);
    
			return parameters;
		  }
	  }

	  private IDictionary<string, string> CompleteExecutedJobDateAsStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters[QUERY_PARAM_EXECUTED_JOB_AFTER] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_AFTER;
			parameters[QUERY_PARAM_EXECUTED_JOB_BEFORE] = MockProvider.EXAMPLE_HISTORIC_PROCESS_INSTANCE_STARTED_BEFORE;
    
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActive()
	  public virtual void testQueryByActive()
	  {
		given().queryParam("active", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).active();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCompleted()
	  public virtual void testQueryByCompleted()
	  {
		given().queryParam("completed", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).completed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBySuspended()
	  public virtual void testQueryBySuspended()
	  {
		given().queryParam("suspended", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).suspended();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExternallyTerminated()
	  public virtual void testQueryByExternallyTerminated()
	  {
		given().queryParam("externallyTerminated", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).externallyTerminated();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInternallyTerminated()
	  public virtual void testQueryByInternallyTerminated()
	  {
		given().queryParam("internallyTerminated", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).internallyTerminated();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTwoStates()
	  public virtual void testQueryByTwoStates()
	  {
		string message = "expected exception";
		doThrow(new BadUserRequestException(message)).when(mockedQuery).completed();

		given().queryParam("active", true).queryParam("completed", true).then().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo(message)).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).active();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActiveAsPost()
	  public virtual void testQueryByActiveAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["active"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).active();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCompletedAsPost()
	  public virtual void testQueryByCompletedAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["completed"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).completed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBySuspendedAsPost()
	  public virtual void testQueryBySuspendedAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["suspended"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).suspended();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExternallyTerminatedAsPost()
	  public virtual void testQueryByExternallyTerminatedAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["externallyTerminated"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).externallyTerminated();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInternallyTerminatedAsPost()
	  public virtual void testQueryByInternallyTerminatedAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["internallyTerminated"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).internallyTerminated();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTwoStatesAsPost()
	  public virtual void testQueryByTwoStatesAsPost()
	  {
		string message = "expected exception";
		doThrow(new BadUserRequestException(message)).when(mockedQuery).completed();

		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["active"] = true;
		parameters["completed"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(BadUserRequestException).Name)).body("message", equalTo(message)).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).active();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootProcessInstances()
	  public virtual void testQueryByRootProcessInstances()
	  {
		given().queryParam("rootProcessInstances", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).rootProcessInstances();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootProcessInstancesAsPost()
	  public virtual void testQueryByRootProcessInstancesAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["rootProcessInstances"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_PROCESS_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).rootProcessInstances();
	  }

	}

}