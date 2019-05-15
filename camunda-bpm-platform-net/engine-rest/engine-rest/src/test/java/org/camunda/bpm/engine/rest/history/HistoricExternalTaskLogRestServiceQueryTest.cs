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
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
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
//	import static junit.framework.TestCase.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	public class HistoricExternalTaskLogRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/external-task-log";
	  protected internal static readonly string HISTORIC_EXTERNAL_TASK_LOG_COUNT_RESOURCE_URL = HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL + "/count";

	  protected internal static readonly long EXTERNAL_TASK_LOG_QUERY_MAX_PRIORITY = long.MaxValue;
	  protected internal static readonly long EXTERNAL_TASK_LOG_QUERY_MIN_PRIORITY = long.MinValue;

	  protected internal HistoricExternalTaskLogQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData() throws java.io.IOException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricExternalTaskLogQuery(MockProvider.createMockHistoricExternalTaskLogs());
	  }

	  protected internal virtual HistoricExternalTaskLogQuery setUpMockHistoricExternalTaskLogQuery(IList<HistoricExternalTaskLog> mockedHistoricExternalTaskLogs)
	  {
		HistoricExternalTaskLogQuery mockedHistoricExternalTaskLogQuery = mock(typeof(HistoricExternalTaskLogQuery));
		when(mockedHistoricExternalTaskLogQuery.list()).thenReturn(mockedHistoricExternalTaskLogs);
		when(mockedHistoricExternalTaskLogQuery.count()).thenReturn((long) mockedHistoricExternalTaskLogs.Count);

		when(processEngine.HistoryService.createHistoricExternalTaskLogQuery()).thenReturn(mockedHistoricExternalTaskLogQuery);

		return mockedHistoricExternalTaskLogQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processDefinitionKey", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

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

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "processDefinitionId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("timestamp", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTimestamp();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("timestamp", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTimestamp();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("externalTaskId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByExternalTaskId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("externalTaskId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByExternalTaskId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("topicName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTopicName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("topicName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTopicName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("workerId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByWorkerId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("workerId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByWorkerId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("retries", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByRetries();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("retries", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByRetries();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("priority", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByPriority();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("priority", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByPriority();
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
		executeAndVerifySorting("activityInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityInstanceId();
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
		executeAndVerifySorting("processInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
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
		json["sorting"] = OrderingBuilder.create().orderBy("processInstanceId").desc().orderBy("timestamp").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByTimestamp();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_EXTERNAL_TASK_LOG_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().body("count", equalTo(1)).when().post(HISTORIC_EXTERNAL_TASK_LOG_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricExternalTaskLogQuery()
	  public virtual void testSimpleHistoricExternalTaskLogQuery()
	  {
		string processInstanceId = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		Response response = given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processInstanceId(processInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> logs = from(content).getList("");
		assertEquals("There should be one historic externalTask log returned.", 1, logs.Count);
		assertNotNull("The returned historic externalTask log should not be null.", logs[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedTimestamp = from(content).getString("[0].timestamp");
		string returnedExternalTaskId = from(content).getString("[0].externalTaskId");
		string returnedExternalTaskTopicName = from(content).getString("[0].topicName");
		string returnedExternalTaskWorkerId = from(content).getString("[0].workerId");
		int returnedRetries = from(content).getInt("[0].retries");
		long returnedPriority = from(content).getLong("[0].priority");
		string returnedErrorMessage = from(content).getString("[0].errorMessage");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedActivityInstanceId = from(content).getString("[0].activityInstanceId");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		bool returnedCreationLog = from(content).getBoolean("[0].creationLog");
		bool returnedFailureLog = from(content).getBoolean("[0].failureLog");
		bool returnedSuccessLog = from(content).getBoolean("[0].successLog");
		bool returnedDeletionLog = from(content).getBoolean("[0].deletionLog");

		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID, returnedId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TIMESTAMP, returnedTimestamp);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID, returnedExternalTaskId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME, returnedExternalTaskTopicName);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID, returnedExternalTaskWorkerId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_RETRIES, returnedRetries);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PRIORITY, returnedPriority);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG, returnedErrorMessage);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_ID, returnedActivityId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_INSTANCE_ID, returnedActivityInstanceId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXECUTION_ID, returnedExecutionId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID, returnedProcessInstanceId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID, returnedProcessDefinitionId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY, returnedProcessDefinitionKey);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG, returnedCreationLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG, returnedFailureLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG, returnedSuccessLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG, returnedDeletionLog);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricExternalTaskLogQueryAsPost()
	  public virtual void testSimpleHistoricExternalTaskLogQueryAsPost()
	  {
		string processInstanceId = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID;

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processInstanceId"] = processInstanceId;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).processInstanceId(processInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> logs = from(content).getList("");
		assertEquals("There should be one historic externalTask log returned.", 1, logs.Count);
		assertNotNull("The returned historic externalTask log should not be null.", logs[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedTimestamp = from(content).getString("[0].timestamp");
		string returnedRemovalTime = from(content).getString("[0].removalTime");
		string returnedExternalTaskId = from(content).getString("[0].externalTaskId");
		string returnedExternalTaskTopicName = from(content).getString("[0].topicName");
		string returnedExternalTaskWorkerId = from(content).getString("[0].workerId");
		int returnedRetries = from(content).getInt("[0].retries");
		long returnedPriority = from(content).getLong("[0].priority");
		string returnedErrorMessage = from(content).getString("[0].errorMessage");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedActivityInstanceId = from(content).getString("[0].activityInstanceId");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedRootProcessInstanceId = from(content).getString("[0].rootProcessInstanceId");
		bool returnedCreationLog = from(content).getBoolean("[0].creationLog");
		bool returnedFailureLog = from(content).getBoolean("[0].failureLog");
		bool returnedSuccessLog = from(content).getBoolean("[0].successLog");
		bool returnedDeletionLog = from(content).getBoolean("[0].deletionLog");

		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID, returnedId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TIMESTAMP, returnedTimestamp);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_REMOVAL_TIME, returnedRemovalTime);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID, returnedExternalTaskId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME, returnedExternalTaskTopicName);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID, returnedExternalTaskWorkerId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_RETRIES, returnedRetries);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PRIORITY, returnedPriority);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG, returnedErrorMessage);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_ID, returnedActivityId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_INSTANCE_ID, returnedActivityInstanceId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXECUTION_ID, returnedExecutionId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID, returnedProcessInstanceId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID, returnedProcessDefinitionId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY, returnedProcessDefinitionKey);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ROOT_PROC_INST_ID, returnedRootProcessInstanceId);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG, returnedCreationLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG, returnedFailureLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG, returnedSuccessLog);
		assertEquals(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG, returnedDeletionLog);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStringParameters()
	  public virtual void testStringParameters()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStringParametersAsPost()
	  public virtual void testStringParametersAsPost()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

	  protected internal virtual IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["logId"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID;
			parameters["externalTaskId"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID;
			parameters["topicName"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME;
			parameters["workerId"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID;
			parameters["errorMessage"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG;
			parameters["processInstanceId"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID;
			parameters["processDefinitionKey"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).logId(stringQueryParameters["logId"]);
		verify(mockedQuery).externalTaskId(stringQueryParameters["externalTaskId"]);
		verify(mockedQuery).topicName(stringQueryParameters["topicName"]);
		verify(mockedQuery).workerId(stringQueryParameters["workerId"]);
		verify(mockedQuery).errorMessage(stringQueryParameters["errorMessage"]);
		verify(mockedQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockedQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionKey(stringQueryParameters["processDefinitionKey"]);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListParameters()
	  public virtual void testListParameters()
	  {
		string anActId = "anActId";
		string anotherActId = "anotherActId";

		string anActInstId = "anActInstId";
		string anotherActInstId = "anotherActInstId";

		string anExecutionId = "anExecutionId";
		string anotherExecutionId = "anotherExecutionId";

		given().queryParam("activityIdIn", anActId + "," + anotherActId).queryParam("activityInstanceIdIn", anActInstId + "," + anotherActInstId).queryParam("executionIdIn", anExecutionId + "," + anotherExecutionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).activityIdIn(anActId, anotherActId);
		verify(mockedQuery).activityInstanceIdIn(anActInstId, anotherActInstId);
		verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListParametersAsPost()
	  public virtual void testListParametersAsPost()
	  {
		string anActId = "anActId";
		string anotherActId = "anotherActId";

		string anActInstId = "anActInstId";
		string anotherActInstId = "anotherActInstId";

		string anExecutionId = "anExecutionId";
		string anotherExecutionId = "anotherExecutionId";

		IDictionary<string, IList<string>> json = new Dictionary<string, IList<string>>();
		json["activityIdIn"] = Arrays.asList(anActId, anotherActId);
		json["activityInstanceIdIn"] = Arrays.asList(anActInstId, anotherActInstId);
		json["executionIdIn"] = Arrays.asList(anExecutionId, anotherExecutionId);

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).activityIdIn(anActId, anotherActId);
		verify(mockedQuery).activityInstanceIdIn(anActInstId, anotherActInstId);
		verify(mockedQuery).executionIdIn(anExecutionId, anotherExecutionId);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBooleanParameters()
	  public virtual void testBooleanParameters()
	  {
		IDictionary<string, bool> @params = CompleteBooleanQueryParameters;

		given().queryParams(@params).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyBooleanParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBooleanParametersAsPost()
	  public virtual void testBooleanParametersAsPost()
	  {
		IDictionary<string, bool> @params = CompleteBooleanQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyBooleanParameterQueryInvocations();

	  }

	  protected internal virtual IDictionary<string, bool> CompleteBooleanQueryParameters
	  {
		  get
		  {
			IDictionary<string, bool> parameters = new Dictionary<string, bool>();
    
			parameters["creationLog"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG;
			parameters["failureLog"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG;
			parameters["successLog"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG;
			parameters["deletionLog"] = MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyBooleanParameterQueryInvocations()
	  {
		verify(mockedQuery).creationLog();
		verify(mockedQuery).failureLog();
		verify(mockedQuery).successLog();
		verify(mockedQuery).deletionLog();
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntegerParameters()
	  public virtual void testIntegerParameters()
	  {
		IDictionary<string, object> @params = CompleteIntegerQueryParameters;

		given().queryParams(@params).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyIntegerParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIntegerParametersAsPost()
	  public virtual void testIntegerParametersAsPost()
	  {
		IDictionary<string, object> @params = CompleteIntegerQueryParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verifyIntegerParameterQueryInvocations();

	  }

	  protected internal virtual IDictionary<string, object> CompleteIntegerQueryParameters
	  {
		  get
		  {
			IDictionary<string, object> parameters = new Dictionary<string, object>();
    
			parameters["priorityLowerThanOrEquals"] = EXTERNAL_TASK_LOG_QUERY_MAX_PRIORITY;
			parameters["priorityHigherThanOrEquals"] = EXTERNAL_TASK_LOG_QUERY_MIN_PRIORITY;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyIntegerParameterQueryInvocations()
	  {
		verify(mockedQuery).priorityLowerThanOrEquals(EXTERNAL_TASK_LOG_QUERY_MAX_PRIORITY);
		verify(mockedQuery).priorityHigherThanOrEquals(EXTERNAL_TASK_LOG_QUERY_MIN_PRIORITY);
		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricExternalTaskLogQuery(createMockHistoricExternalTaskLogsTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> externalTaskLogs = from(content).getList("");
		assertThat(externalTaskLogs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockedQuery = setUpMockHistoricExternalTaskLogQuery(createMockHistoricExternalTaskLogsTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> externalTaskLogs = from(content).getList("");
		assertThat(externalTaskLogs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

	  private IList<HistoricExternalTaskLog> createMockHistoricExternalTaskLogsTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricExternalTaskLog(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricExternalTaskLog(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }


	}

}