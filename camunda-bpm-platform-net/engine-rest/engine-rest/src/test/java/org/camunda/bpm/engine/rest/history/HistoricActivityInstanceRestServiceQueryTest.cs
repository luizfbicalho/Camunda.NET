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
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
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

	public class HistoricActivityInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/activity-instance";

	  protected internal static readonly string HISTORIC_ACTIVITY_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricActivityInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricActivityInstanceQuery(MockProvider.createMockHistoricActivityInstances());
	  }

	  private HistoricActivityInstanceQuery setUpMockHistoricActivityInstanceQuery(IList<HistoricActivityInstance> mockedHistoricActivityInstances)
	  {
		HistoricActivityInstanceQuery mockedhistoricActivityInstanceQuery = mock(typeof(HistoricActivityInstanceQuery));
		when(mockedhistoricActivityInstanceQuery.list()).thenReturn(mockedHistoricActivityInstances);
		when(mockedhistoricActivityInstanceQuery.count()).thenReturn((long) mockedHistoricActivityInstances.Count);

		when(processEngine.HistoryService.createHistoricActivityInstanceQuery()).thenReturn(mockedhistoricActivityInstanceQuery);

		return mockedhistoricActivityInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "instanceId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("instanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("instanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("executionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByExecutionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("executionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByExecutionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityType();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("startTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceStartTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("startTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceStartTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceEndTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceEndTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceDuration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceDuration();
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
		executeAndVerifySorting("occurrence", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderPartiallyByOccurrence();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("occurrence", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderPartiallyByOccurrence();
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
		json["sorting"] = OrderingBuilder.create().orderBy("definitionId").desc().orderBy("instanceId").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
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

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_ACTIVITY_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().body("count", equalTo(1)).when().post(HISTORIC_ACTIVITY_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricActivityQuery()
	  public virtual void testSimpleHistoricActivityQuery()
	  {
		string processInstanceId = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		Response response = given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processInstanceId(processInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one activity instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned activity instance should not be null.", instances[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedParentActivityInstanceId = from(content).getString("[0].parentActivityInstanceId");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedActivityName = from(content).getString("[0].activityName");
		string returnedActivityType = from(content).getString("[0].activityType");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedTaskId = from(content).getString("[0].taskId");
		string returnedCalledProcessInstanceId = from(content).getString("[0].calledProcessInstanceId");
		string returnedCalledCaseInstanceId = from(content).getString("[0].calledCaseInstanceId");
		string returnedAssignee = from(content).getString("[0].assignee");
		DateTime returnedStartTime = DateTimeUtil.parseDate(from(content).getString("[0].startTime"));
		DateTime returnedEndTime = DateTimeUtil.parseDate(from(content).getString("[0].endTime"));
		long returnedDurationInMillis = from(content).getLong("[0].durationInMillis");
		bool canceled = from(content).getBoolean("[0].canceled");
		bool completeScope = from(content).getBoolean("[0].completeScope");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_PARENT_ACTIVITY_INSTANCE_ID, returnedParentActivityInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_ID, returnedActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_NAME, returnedActivityName);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_TYPE, returnedActivityType);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ID, returnedTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID, returnedCalledProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID, returnedCalledCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME, returnedAssignee);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME), returnedStartTime);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME), returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_DURATION, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_CANCELED, canceled);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_COMPLETE_SCOPE, completeScope);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersAsPost()
	  public virtual void testAdditionalParametersAsPost()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBooleanParameters()
	  public virtual void testBooleanParameters()
	  {
		IDictionary<string, bool> @params = CompleteBooleanQueryParameters;

		given().queryParams(@params).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyBooleanParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBooleanParametersAsPost()
	  public virtual void testBooleanParametersAsPost()
	  {
		IDictionary<string, bool> @params = CompleteBooleanQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyBooleanParameterQueryInvocations();
	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["activityInstanceId"] = MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID;
			parameters["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
			parameters["executionId"] = MockProvider.EXAMPLE_EXECUTION_ID;
			parameters["activityId"] = MockProvider.EXAMPLE_ACTIVITY_ID;
			parameters["activityName"] = MockProvider.EXAMPLE_ACTIVITY_NAME;
			parameters["activityType"] = MockProvider.EXAMPLE_ACTIVITY_TYPE;
			parameters["taskAssignee"] = MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME;
    
			return parameters;
		  }
	  }

	  private IDictionary<string, bool> CompleteBooleanQueryParameters
	  {
		  get
		  {
			IDictionary<string, bool> parameters = new Dictionary<string, bool>();
    
			parameters["canceled"] = MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_CANCELED;
			parameters["completeScope"] = MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_COMPLETE_SCOPE;
    
			return parameters;
		  }
	  }

	  private void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).activityInstanceId(stringQueryParameters["activityInstanceId"]);
		verify(mockedQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockedQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockedQuery).executionId(stringQueryParameters["executionId"]);
		verify(mockedQuery).activityId(stringQueryParameters["activityId"]);
		verify(mockedQuery).activityName(stringQueryParameters["activityName"]);
		verify(mockedQuery).activityType(stringQueryParameters["activityType"]);
		verify(mockedQuery).taskAssignee(stringQueryParameters["taskAssignee"]);

		verify(mockedQuery).list();
	  }

	  private void verifyBooleanParameterQueryInvocations()
	  {
		IDictionary<string, bool> booleanParams = CompleteBooleanQueryParameters;
		bool? canceled = booleanParams["canceled"];
		bool? completeScope = booleanParams["completeScope"];

		if (canceled != null && canceled)
		{
		  verify(mockedQuery).canceled();
		}
		if (completeScope != null && completeScope)
		{
		  verify(mockedQuery).completeScope();
		}

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFinishedHistoricActivityQuery()
	  public virtual void testFinishedHistoricActivityQuery()
	  {
		Response response = given().queryParam("finished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).finished();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one activity instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned activity instance should not be null.", instances[0]);

		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME, returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFinishedHistoricActivityQueryAsPost()
	  public virtual void testFinishedHistoricActivityQueryAsPost()
	  {
		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["finished"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).finished();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one activity instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned activity instance should not be null.", instances[0]);

		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME, returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnfinishedHistoricActivityQuery()
	  public virtual void testUnfinishedHistoricActivityQuery()
	  {
		IList<HistoricActivityInstance> mockedHistoricActivityInstances = MockProvider.createMockRunningHistoricActivityInstances();
		HistoricActivityInstanceQuery mockedhistoricActivityInstanceQuery = mock(typeof(HistoricActivityInstanceQuery));
		when(mockedhistoricActivityInstanceQuery.list()).thenReturn(mockedHistoricActivityInstances);
		when(processEngine.HistoryService.createHistoricActivityInstanceQuery()).thenReturn(mockedhistoricActivityInstanceQuery);

		Response response = given().queryParam("unfinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedhistoricActivityInstanceQuery);
		inOrder.verify(mockedhistoricActivityInstanceQuery).unfinished();
		inOrder.verify(mockedhistoricActivityInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one activity instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned activity instance should not be null.", instances[0]);

		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertNull(returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnfinishedHistoricActivityQueryAsPost()
	  public virtual void testUnfinishedHistoricActivityQueryAsPost()
	  {
		IList<HistoricActivityInstance> mockedHistoricActivityInstances = MockProvider.createMockRunningHistoricActivityInstances();
		HistoricActivityInstanceQuery mockedhistoricActivityInstanceQuery = mock(typeof(HistoricActivityInstanceQuery));
		when(mockedhistoricActivityInstanceQuery.list()).thenReturn(mockedHistoricActivityInstances);
		when(processEngine.HistoryService.createHistoricActivityInstanceQuery()).thenReturn(mockedhistoricActivityInstanceQuery);

		IDictionary<string, bool> body = new Dictionary<string, bool>();
		body["unfinished"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(body).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedhistoricActivityInstanceQuery);
		inOrder.verify(mockedhistoricActivityInstanceQuery).unfinished();
		inOrder.verify(mockedhistoricActivityInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one activity instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned activity instance should not be null.", instances[0]);

		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertNull(returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeQuery()
	  public virtual void testHistoricBeforeAndAfterStartTimeQuery()
	  {
		given().queryParam("startedBefore", MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_BEFORE).queryParam("startedAfter", MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_AFTER).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyStartParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricBeforeAndAfterStartTimeQueryAsPost()
	  public virtual void testHistoricBeforeAndAfterStartTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteStartDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyStartParameterQueryInvocations();
	  }

	  private IDictionary<string, DateTime> CompleteStartDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["startedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_AFTER);
			parameters["startedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_STARTED_BEFORE);
    
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeFinishTimeQuery()
	  public virtual void testHistoricAfterAndBeforeFinishTimeQuery()
	  {
		given().queryParam("finishedAfter", MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_AFTER).queryParam("finishedBefore", MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyFinishedParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeFinishTimeQueryAsPost()
	  public virtual void testHistoricAfterAndBeforeFinishTimeQueryAsPost()
	  {
		IDictionary<string, DateTime> parameters = CompleteFinishedDateQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyFinishedParameterQueryInvocations();
	  }

	  private IDictionary<string, DateTime> CompleteFinishedDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["finishedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_AFTER);
			parameters["finishedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_FINISHED_BEFORE);
    
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

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricActivityInstanceQuery(createMockHistoricActivityInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

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
		mockedQuery = setUpMockHistoricActivityInstanceQuery(createMockHistoricActivityInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_ACTIVITY_INSTANCE_RESOURCE_URL);

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

	  private IList<HistoricActivityInstance> createMockHistoricActivityInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricActivityInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricActivityInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }
	}

}