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
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using ExternalTaskQuery = org.camunda.bpm.engine.externaltask.ExternalTaskQuery;
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

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ExternalTaskRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string EXTERNAL_TASK_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/external-task";
	  protected internal static readonly string EXTERNAL_TASK_COUNT_QUERY_URL = EXTERNAL_TASK_QUERY_URL + "/count";
	  public const long EXTERNAL_TASK_LOW_BOUND_PRIORITY = 3L;
	  public const long EXTERNAL_TASK_HIGH_BOUND_PRIORITY = 4L;

	  protected internal ExternalTaskQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = setUpMockExternalTaskQuery(MockProvider.createMockExternalTasks());
	  }

	  private ExternalTaskQuery setUpMockExternalTaskQuery(IList<ExternalTask> mockedTasks)
	  {
		ExternalTaskQuery sampleTaskQuery = mock(typeof(ExternalTaskQuery));
		when(sampleTaskQuery.list()).thenReturn(mockedTasks);
		when(sampleTaskQuery.count()).thenReturn((long) mockedTasks.Count);

		when(processEngine.ExternalTaskService.createExternalTaskQuery()).thenReturn(sampleTaskQuery);

		return sampleTaskQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDateParameter()
	  public virtual void testInvalidDateParameter()
	  {
		given().queryParams("lockExpirationBefore", "anInvalidDate").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot set query parameter 'lockExpirationBefore' to value 'anInvalidDate': " + "Cannot convert value \"anInvalidDate\" to java type java.util.Date")).when().get(EXTERNAL_TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "processInstanceId").header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(EXTERNAL_TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(EXTERNAL_TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleTaskQuery()
	  public virtual void testSimpleTaskQuery()
	  {
		Response response = given().header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		Mockito.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one external task returned.", 1, instances.Count);
		Assert.assertNotNull("The returned external task should not be null.", instances[0]);

		string activityId = from(content).getString("[0].activityId");
		string activityInstanceId = from(content).getString("[0].activityInstanceId");
		string errorMessage = from(content).getString("[0].errorMessage");
		string executionId = from(content).getString("[0].executionId");
		string id = from(content).getString("[0].id");
		string lockExpirationTime = from(content).getString("[0].lockExpirationTime");
		string processDefinitionId = from(content).getString("[0].processDefinitionId");
		string processDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string processInstanceId = from(content).getString("[0].processInstanceId");
		int? retries = from(content).getInt("[0].retries");
		bool? suspended = from(content).getBoolean("[0].suspended");
		string topicName = from(content).getString("[0].topicName");
		string workerId = from(content).getString("[0].workerId");
		string tenantId = from(content).getString("[0].tenantId");
		long priority = from(content).getLong("[0].priority");
		string businessKey = from(content).getString("[0].businessKey");

		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_ID, activityId);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_INSTANCE_ID, activityInstanceId);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE, errorMessage);
		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, executionId);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_ID, id);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_LOCK_EXPIRATION_TIME, lockExpirationTime);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, processDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, processDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, processInstanceId);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_RETRIES, retries);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_SUSPENDED, suspended);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_TOPIC_NAME, topicName);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_WORKER_ID, workerId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, tenantId);
		Assert.assertEquals(MockProvider.EXTERNAL_TASK_PRIORITY, priority);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY, businessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompleteGETQuery()
	  public virtual void testCompleteGETQuery()
	  {
		IDictionary<string, string> parameters = new Dictionary<string, string>();

		parameters["externalTaskId"] = "someExternalTaskId";
		parameters["activityId"] = "someActivityId";
		parameters["lockExpirationBefore"] = withTimezone("2013-01-23T14:42:42");
		parameters["lockExpirationAfter"] = withTimezone("2013-01-23T15:52:52");
		parameters["topicName"] = "someTopic";
		parameters["locked"] = "true";
		parameters["notLocked"] = "true";
		parameters["executionId"] = "someExecutionId";
		parameters["processInstanceId"] = "someProcessInstanceId";
		parameters["processInstanceIdIn"] = "aProcessInstanceId,anotherProcessInstanceId";
		parameters["processDefinitionId"] = "someProcessDefinitionId";
		parameters["active"] = "true";
		parameters["suspended"] = "true";
		parameters["withRetriesLeft"] = "true";
		parameters["noRetriesLeft"] = "true";
		parameters["workerId"] = "someWorkerId";
		parameters["priorityHigherThanOrEquals"] = "3";
		parameters["priorityLowerThanOrEquals"] = "4";

		given().queryParams(parameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).externalTaskId("someExternalTaskId");
		verify(mockQuery).activityId("someActivityId");
		verify(mockQuery).lockExpirationBefore(DateTimeUtil.parseDate(withTimezone("2013-01-23T14:42:42")));
		verify(mockQuery).lockExpirationAfter(DateTimeUtil.parseDate(withTimezone("2013-01-23T15:52:52")));
		verify(mockQuery).topicName("someTopic");
		verify(mockQuery).locked();
		verify(mockQuery).notLocked();
		verify(mockQuery).executionId("someExecutionId");
		verify(mockQuery).processInstanceId("someProcessInstanceId");
		verify(mockQuery).processInstanceIdIn("aProcessInstanceId", "anotherProcessInstanceId");
		verify(mockQuery).processDefinitionId("someProcessDefinitionId");
		verify(mockQuery).active();
		verify(mockQuery).suspended();
		verify(mockQuery).withRetriesLeft();
		verify(mockQuery).noRetriesLeft();
		verify(mockQuery).workerId("someWorkerId");
		verify(mockQuery).priorityHigherThanOrEquals(3);
		verify(mockQuery).priorityLowerThanOrEquals(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletePOSTQuery()
	  public virtual void testCompletePOSTQuery()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();

		parameters["externalTaskId"] = "someExternalTaskId";
		parameters["activityId"] = "someActivityId";
		parameters["lockExpirationBefore"] = withTimezone("2013-01-23T14:42:42");
		parameters["lockExpirationAfter"] = withTimezone("2013-01-23T15:52:52");
		parameters["topicName"] = "someTopic";
		parameters["locked"] = "true";
		parameters["notLocked"] = "true";
		parameters["executionId"] = "someExecutionId";
		parameters["processInstanceId"] = "someProcessInstanceId";
		parameters["processInstanceIdIn"] = Arrays.asList("aProcessInstanceId", "anotherProcessInstanceId");
		parameters["processDefinitionId"] = "someProcessDefinitionId";
		parameters["active"] = "true";
		parameters["suspended"] = "true";
		parameters["withRetriesLeft"] = "true";
		parameters["noRetriesLeft"] = "true";
		parameters["workerId"] = "someWorkerId";
		parameters["priorityHigherThanOrEquals"] = "3";
		parameters["priorityLowerThanOrEquals"] = "4";

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).externalTaskId("someExternalTaskId");
		verify(mockQuery).activityId("someActivityId");
		verify(mockQuery).lockExpirationBefore(DateTimeUtil.parseDate(withTimezone("2013-01-23T14:42:42")));
		verify(mockQuery).lockExpirationAfter(DateTimeUtil.parseDate(withTimezone("2013-01-23T15:52:52")));
		verify(mockQuery).topicName("someTopic");
		verify(mockQuery).locked();
		verify(mockQuery).notLocked();
		verify(mockQuery).executionId("someExecutionId");
		verify(mockQuery).processInstanceId("someProcessInstanceId");
		verify(mockQuery).processInstanceIdIn("aProcessInstanceId", "anotherProcessInstanceId");
		verify(mockQuery).processDefinitionId("someProcessDefinitionId");
		verify(mockQuery).active();
		verify(mockQuery).suspended();
		verify(mockQuery).withRetriesLeft();
		verify(mockQuery).noRetriesLeft();
		verify(mockQuery).workerId("someWorkerId");
		verify(mockQuery).priorityHigherThanOrEquals(3);
		verify(mockQuery).priorityLowerThanOrEquals(4);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGETQuerySorting()
	  public virtual void testGETQuerySorting()
	  {
		// desc
		InOrder inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("id", "desc", Status.OK);
		inOrder.verify(mockQuery).orderById();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("lockExpirationTime", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByLockExpirationTime();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processInstanceId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessInstanceId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processDefinitionId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessDefinitionId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("taskPriority", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByPriority();
		inOrder.verify(mockQuery).desc();
		// asc
		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("id", "asc", Status.OK);
		inOrder.verify(mockQuery).orderById();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("lockExpirationTime", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByLockExpirationTime();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processInstanceId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessInstanceId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processDefinitionId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessDefinitionId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("processDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyGETSorting("taskPriority", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByPriority();
		inOrder.verify(mockQuery).asc();
	  }

	  protected internal virtual void executeAndVerifyGETSorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(expectedStatus.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPOSTQuerySorting()
	  public virtual void testPOSTQuerySorting()
	  {
		InOrder inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifyPOSTSorting(OrderingBuilder.create().orderBy("processDefinitionKey").desc().orderBy("lockExpirationTime").asc().Json, Status.OK);

		inOrder.verify(mockQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockQuery).desc();
		inOrder.verify(mockQuery).orderByLockExpirationTime();
		inOrder.verify(mockQuery).asc();
	  }

	  protected internal virtual void executeAndVerifyPOSTSorting(IList<IDictionary<string, object>> sortingJson, Status expectedStatus)
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = sortingJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaginationGET()
	  public virtual void testPaginationGET()
	  {
		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPaginationPOST()
	  public virtual void testPaginationPOST()
	  {
		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGETQueryCount()
	  public virtual void testGETQueryCount()
	  {
		given().header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(EXTERNAL_TASK_COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPOSTQueryCount()
	  public virtual void testPOSTQueryCount()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(EXTERNAL_TASK_COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIdListGet()
	  public virtual void testQueryByTenantIdListGet()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockExternalTasksTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIdListPost()
	  public virtual void testQueryByTenantIdListPost()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockExternalTasksTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

	  private IList<ExternalTask> createMockExternalTasksTwoTenants()
	  {
		return Arrays.asList(MockProvider.mockExternalTask().buildExternalTask(), MockProvider.mockExternalTask().tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).buildExternalTask());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityIdListGet()
	  public virtual void testQueryByActivityIdListGet()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockExternalTasksTwoActivityIds());

		Response response = given().queryParam("activityIdIn", MockProvider.EXAMPLE_ACTIVITY_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).activityIdIn(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedActivityId1 = from(content).getString("[0].activityId");
		string returnedActivityId2 = from(content).getString("[1].activityId");

		assertThat(returnedActivityId1).isEqualTo(MockProvider.EXAMPLE_ACTIVITY_ID);
		assertThat(returnedActivityId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityIdListPost()
	  public virtual void testQueryByActivityIdListPost()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockExternalTasksTwoActivityIds());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["activityIdIn"] = MockProvider.EXAMPLE_ACTIVITY_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).activityIdIn(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		string returnedActivityId1 = from(content).getString("[0].activityId");
		string returnedActivityId2 = from(content).getString("[1].activityId");

		assertThat(returnedActivityId1).isEqualTo(MockProvider.EXAMPLE_ACTIVITY_ID);
		assertThat(returnedActivityId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID);
	  }

	  private IList<ExternalTask> createMockExternalTasksTwoActivityIds()
	  {
		return Arrays.asList(MockProvider.mockExternalTask().buildExternalTask(), MockProvider.mockExternalTask().activityId(MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID).buildExternalTask());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByPriorityListGet()
	  public virtual void testQueryByPriorityListGet()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockedExternalTasksWithPriorities());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["priorityHigherThanOrEquals"] = "3";
		queryParameters["priorityLowerThanOrEquals"] = "4";

		Response response = given().queryParameters(queryParameters).expect().statusCode(Status.OK.StatusCode).when().get(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).priorityHigherThanOrEquals(EXTERNAL_TASK_LOW_BOUND_PRIORITY);
		verify(mockQuery).priorityLowerThanOrEquals(EXTERNAL_TASK_HIGH_BOUND_PRIORITY);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		long prio1 = from(content).getLong("[0].priority");
		long prio2 = from(content).getLong("[1].priority");

		assertThat(prio1).isEqualTo(EXTERNAL_TASK_LOW_BOUND_PRIORITY);
		assertThat(prio2).isEqualTo(EXTERNAL_TASK_HIGH_BOUND_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByPriorityListPost()
	  public virtual void testQueryByPriorityListPost()
	  {
		mockQuery = setUpMockExternalTaskQuery(createMockedExternalTasksWithPriorities());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["priorityHigherThanOrEquals"] = "3";
		queryParameters["priorityLowerThanOrEquals"] = "4";

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(EXTERNAL_TASK_QUERY_URL);

		verify(mockQuery).priorityHigherThanOrEquals(EXTERNAL_TASK_LOW_BOUND_PRIORITY);
		verify(mockQuery).priorityLowerThanOrEquals(EXTERNAL_TASK_HIGH_BOUND_PRIORITY);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> executions = from(content).getList("");
		assertThat(executions).hasSize(2);

		long prio1 = from(content).getLong("[0].priority");
		long prio2 = from(content).getLong("[1].priority");

		assertThat(prio1).isEqualTo(EXTERNAL_TASK_LOW_BOUND_PRIORITY);
		assertThat(prio2).isEqualTo(EXTERNAL_TASK_HIGH_BOUND_PRIORITY);
	  }


	  private IList<ExternalTask> createMockedExternalTasksWithPriorities()
	  {
		return Arrays.asList(MockProvider.mockExternalTask().priority(EXTERNAL_TASK_LOW_BOUND_PRIORITY).buildExternalTask(), MockProvider.mockExternalTask().priority(EXTERNAL_TASK_HIGH_BOUND_PRIORITY).buildExternalTask());
	  }
	}

}