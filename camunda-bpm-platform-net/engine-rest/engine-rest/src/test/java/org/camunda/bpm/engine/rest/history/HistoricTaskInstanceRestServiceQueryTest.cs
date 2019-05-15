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
//	import static org.camunda.bpm.engine.rest.util.QueryParamUtils.arrayAsCommaSeperatedList;
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



	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
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

	public class HistoricTaskInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_TASK_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/task";
	  protected internal static readonly string HISTORIC_TASK_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_TASK_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricTaskInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricTaskInstanceQuery(MockProvider.createMockHistoricTaskInstances());
	  }

	  private HistoricTaskInstanceQuery setUpMockHistoricTaskInstanceQuery(IList<HistoricTaskInstance> mockedHistoricTaskInstances)
	  {
		mockedQuery = mock(typeof(HistoricTaskInstanceQuery));

		when(mockedQuery.list()).thenReturn(mockedHistoricTaskInstances);
		when(mockedQuery.count()).thenReturn((long) mockedHistoricTaskInstances.Count);

		when(processEngine.HistoryService.createHistoricTaskInstanceQuery()).thenReturn(mockedQuery);

		return mockedQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQueryAsPost()
	  public virtual void testNoParametersQueryAsPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).list();
		verifyNoMoreInteractions(mockedQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortingOptions()
	  public virtual void testInvalidSortingOptions()
	  {
		executeAndVerifySorting("anInvalidSortByOption", "asc", Status.BAD_REQUEST);
		executeAndVerifySorting("processInstanceId", "anInvalidSortOrderOption", Status.BAD_REQUEST);
	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricActivityInstanceId();
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
		executeAndVerifySorting("processInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processInstanceId", "desc", Status.OK);
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
		executeAndVerifySorting("duration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricTaskInstanceDuration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricTaskInstanceDuration();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricTaskInstanceEndTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricTaskInstanceEndTime();
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
		executeAndVerifySorting("taskName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskName();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskDescription", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDescription();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskDescription", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDescription();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("assignee", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskAssignee();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("assignee", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskAssignee();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("owner", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskOwner();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("owner", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskOwner();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("dueDate", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDueDate();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("dueDate", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDueDate();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("followUpDate", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskFollowUpDate();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("followUpDate", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskFollowUpDate();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("deleteReason", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeleteReason();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("deleteReason", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByDeleteReason();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("priority", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskPriority();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("priority", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskPriority();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseDefinitionId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseInstanceId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseExecutionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseExecutionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseExecutionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseExecutionId();
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
		json["sorting"] = OrderingBuilder.create().orderBy("owner").desc().orderBy("priority").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		inOrder.verify(mockedQuery).orderByTaskOwner();
		inOrder.verify(mockedQuery).desc();
		inOrder.verify(mockedQuery).orderByTaskPriority();
		inOrder.verify(mockedQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_TASK_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().body("count", equalTo(1)).when().post(HISTORIC_TASK_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricTaskInstanceQuery()
	  public virtual void testSimpleHistoricTaskInstanceQuery()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one historic task instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned historic task instance should not be null.", instances[0]);

		verifyHistoricTaskInstanceEntries(content);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricTaskInstanceQueryAsPost()
	  public virtual void testSimpleHistoricTaskInstanceQueryAsPost()
	  {
		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one historic task instance returned.", 1, instances.Count);
		Assert.assertNotNull("The returned historic task instance should not be null.", instances[0]);

		verifyHistoricTaskInstanceEntries(content);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskId()
	  public virtual void testQueryByTaskId()
	  {
		string taskId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ID;

		given().queryParam("taskId", taskId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskId(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskIdAsPost()
	  public virtual void testQueryByTaskIdAsPost()
	  {
		string taskId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskId"] = taskId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskId(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		string processInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_ID;

		given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceId(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceIdAsPost()
	  public virtual void testQueryByProcessInstanceIdAsPost()
	  {
		string processInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processInstanceId"] = processInstanceId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceId(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKey()
	  public virtual void testQueryByProcessInstanceBusinessKey()
	  {
		string processInstanceBusinessKey = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_BUSINESS_KEY;

		given().queryParam("processInstanceBusinessKey", processInstanceBusinessKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKey(processInstanceBusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKeyAsPost()
	  public virtual void testQueryByProcessInstanceBusinessKeyAsPost()
	  {
		string processInstanceBusinessKey = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_BUSINESS_KEY;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processInstanceBusinessKey"] = processInstanceBusinessKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKey(processInstanceBusinessKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKeyLike()
	  public virtual void testQueryByProcessInstanceBusinessKeyLike()
	  {
		string processInstanceBusinessKeyLike = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_BUSINESS_KEY;

		given().queryParam("processInstanceBusinessKeyLike", processInstanceBusinessKeyLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKeyLike(processInstanceBusinessKeyLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKeyLikeAsPost()
	  public virtual void testQueryByProcessInstanceBusinessKeyLikeAsPost()
	  {
		string processInstanceBusinessKeyLike = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_BUSINESS_KEY;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processInstanceBusinessKeyLike"] = processInstanceBusinessKeyLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKeyLike(processInstanceBusinessKeyLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKeyIn()
	  public virtual void testQueryByProcessInstanceBusinessKeyIn()
	  {
		given().queryParam("processInstanceBusinessKeyIn", arrayAsCommaSeperatedList("aBusinessKey", "anotherBusinessKey")).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKeyIn("aBusinessKey", "anotherBusinessKey");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceBusinessKeyInAsPost()
	  public virtual void testQueryByProcessInstanceBusinessKeyInAsPost()
	  {
		string businessKey1 = "aBusinessKey";
		string businessKey2 = "anotherBusinessKey";
		IList<string> processInstanceBusinessKeyIn = new List<string>();
		processInstanceBusinessKeyIn.Add(businessKey1);
		processInstanceBusinessKeyIn.Add(businessKey2);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processInstanceBusinessKeyIn"] = processInstanceBusinessKeyIn;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processInstanceBusinessKeyIn(businessKey1, businessKey2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionId()
	  public virtual void testQueryByExecutionId()
	  {
		string executionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_EXEC_ID;

		given().queryParam("executionId", executionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).executionId(executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionIdAsPost()
	  public virtual void testQueryByExecutionIdAsPost()
	  {
		string executionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_EXEC_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["executionId"] = executionId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).executionId(executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceId()
	  public virtual void testQueryByActivityInstanceId()
	  {
		string activityInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID;

		given().queryParam("activityInstanceIdIn", activityInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activityInstanceIdIn(activityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceIdAsPost()
	  public virtual void testQueryByActivityInstanceIdAsPost()
	  {
		string activityInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID;

		IList<string> activityInstanceIds = new List<string>();
		activityInstanceIds.Add(activityInstanceId);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["activityInstanceIdIn"] = activityInstanceIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activityInstanceIdIn(activityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceIds()
	  public virtual void testQueryByActivityInstanceIds()
	  {
		string activityInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID;
		string anotherActivityInstanceId = "anotherActivityInstanceId";

		given().queryParam("activityInstanceIdIn", activityInstanceId + "," + anotherActivityInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activityInstanceIdIn(activityInstanceId, anotherActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityInstanceIdsAsPost()
	  public virtual void testQueryByActivityInstanceIdsAsPost()
	  {
		string activityInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID;
		string anotherActivityInstanceId = "anotherActivityInstanceId";

		IList<string> activityInstanceIds = new List<string>();
		activityInstanceIds.Add(activityInstanceId);
		activityInstanceIds.Add(anotherActivityInstanceId);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["activityInstanceIdIn"] = activityInstanceIds;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).activityInstanceIdIn(activityInstanceId, anotherActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionId()
	  public virtual void testQueryByProcessDefinitionId()
	  {
		string processDefinitionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_ID;

		given().queryParam("processDefinitionId", processDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionId(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionIdAsPost()
	  public virtual void testQueryByProcessDefinitionIdAsPost()
	  {
		string processDefinitionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processDefinitionId"] = processDefinitionId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionId(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKey()
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		string processDefinitionKey = "aProcDefKey";

		given().queryParam("processDefinitionKey", processDefinitionKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionKey(processDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKeyAsPost()
	  public virtual void testQueryByProcessDefinitionKeyAsPost()
	  {
		string processDefinitionKey = "aProcDefKey";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processDefinitionKey"] = processDefinitionKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionKey(processDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionName()
	  public virtual void testQueryByProcessDefinitionName()
	  {
		string processDefinitionName = "aProcDefName";

		given().queryParam("processDefinitionName", processDefinitionName).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionName(processDefinitionName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionNameAsPost()
	  public virtual void testQueryByProcessDefinitionNameAsPost()
	  {
		string processDefinitionName = "aProcDefName";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processDefinitionName"] = processDefinitionName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processDefinitionName(processDefinitionName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskName()
	  public virtual void testQueryByTaskName()
	  {
		string taskName = "aTaskName";

		given().queryParam("taskName", taskName).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskName(taskName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskNameAsPost()
	  public virtual void testQueryByTaskNameAsPost()
	  {
		string taskName = "aTaskName";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskName"] = taskName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskName(taskName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskNameLike()
	  public virtual void testQueryByTaskNameLike()
	  {
		string taskNameLike = "aTaskNameLike";

		given().queryParam("taskNameLike", taskNameLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskNameLike(taskNameLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskNameLikeAsPost()
	  public virtual void testQueryByTaskNameLikeAsPost()
	  {
		string taskNameLike = "aTaskNameLike";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskNameLike"] = taskNameLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskNameLike(taskNameLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDescription()
	  public virtual void testQueryByTaskDescription()
	  {
		string taskDescription = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DESCRIPTION;

		given().queryParam("taskDescription", taskDescription).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDescription(taskDescription);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDescriptionAsPost()
	  public virtual void testQueryByTaskDescriptionAsPost()
	  {
		string taskDescription = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DESCRIPTION;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDescription"] = taskDescription;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDescription(taskDescription);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDescriptionLike()
	  public virtual void testQueryByTaskDescriptionLike()
	  {
		string taskDescriptionLike = "aTaskDescriptionLike";

		given().queryParam("taskDescriptionLike", taskDescriptionLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDescriptionLike(taskDescriptionLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDescriptionLikeAsPost()
	  public virtual void testQueryByTaskDescriptionLikeAsPost()
	  {
		string taskDescriptionLike = "aTaskDescriptionLike";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDescriptionLike"] = taskDescriptionLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDescriptionLike(taskDescriptionLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDefinitionKey()
	  public virtual void testQueryByTaskDefinitionKey()
	  {
		string taskDefinitionKey = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DEF_KEY;

		given().queryParam("taskDefinitionKey", taskDefinitionKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDefinitionKey(taskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDefinitionKeyAsPost()
	  public virtual void testQueryByTaskDefinitionKeyAsPost()
	  {
		string taskDefinitionKey = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DEF_KEY;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDefinitionKey"] = taskDefinitionKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDefinitionKey(taskDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDeleteReason()
	  public virtual void testQueryByTaskDeleteReason()
	  {
		string taskDeleteReason = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DELETE_REASON;

		given().queryParam("taskDeleteReason", taskDeleteReason).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDeleteReason(taskDeleteReason);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDeleteReasonAsPost()
	  public virtual void testQueryByTaskDeleteReasonAsPost()
	  {
		string taskDeleteReason = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DELETE_REASON;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDeleteReason"] = taskDeleteReason;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDeleteReason(taskDeleteReason);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDeleteReasonLike()
	  public virtual void testQueryByTaskDeleteReasonLike()
	  {
		string taskDeleteReasonLike = "aTaskDeleteReasonLike";

		given().queryParam("taskDeleteReasonLike", taskDeleteReasonLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDeleteReasonLike(taskDeleteReasonLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDeleteReasonLikeAsPost()
	  public virtual void testQueryByTaskDeleteReasonLikeAsPost()
	  {
		string taskDeleteReasonLike = "aTaskDeleteReasonLike";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDeleteReasonLike"] = taskDeleteReasonLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDeleteReasonLike(taskDeleteReasonLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskAssignee()
	  public virtual void testQueryByTaskAssignee()
	  {
		string taskAssignee = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ASSIGNEE;

		given().queryParam("taskAssignee", taskAssignee).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssignee(taskAssignee);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskAssigneeAsPost()
	  public virtual void testQueryByTaskAssigneeAsPost()
	  {
		string taskAssignee = MockProvider.EXAMPLE_HISTORIC_TASK_INST_ASSIGNEE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskAssignee"] = taskAssignee;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssignee(taskAssignee);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskAssigneeLike()
	  public virtual void testQueryByTaskAssigneeLike()
	  {
		string taskAssigneeLike = "aTaskAssigneeLike";

		given().queryParam("taskAssigneeLike", taskAssigneeLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssigneeLike(taskAssigneeLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskAssigneeLikeAsPost()
	  public virtual void testQueryByTaskAssigneeLikeAsPost()
	  {
		string taskAssigneeLike = "aTaskAssigneeLike";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskAssigneeLike"] = taskAssigneeLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssigneeLike(taskAssigneeLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskOwner()
	  public virtual void testQueryByTaskOwner()
	  {
		string taskOwner = MockProvider.EXAMPLE_HISTORIC_TASK_INST_OWNER;

		given().queryParam("taskOwner", taskOwner).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskOwner(taskOwner);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskOwnerAsPost()
	  public virtual void testQueryByTaskOwnerAsPost()
	  {
		string taskOwner = MockProvider.EXAMPLE_HISTORIC_TASK_INST_OWNER;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskOwner"] = taskOwner;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskOwner(taskOwner);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskOwnerLike()
	  public virtual void testQueryByTaskOwnerLike()
	  {
		string taskOwnerLike = "aTaskOwnerLike";

		given().queryParam("taskOwnerLike", taskOwnerLike).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskOwnerLike(taskOwnerLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskOwnerLikeAsPost()
	  public virtual void testQueryByTaskOwnerLikeAsPost()
	  {
		string taskOwnerLike = "aTaskOwnerLike";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskOwnerLike"] = taskOwnerLike;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskOwnerLike(taskOwnerLike);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskPriority()
	  public virtual void testQueryByTaskPriority()
	  {
		int taskPriority = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PRIORITY;

		given().queryParam("taskPriority", taskPriority).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskPriority(taskPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskPriorityAsPost()
	  public virtual void testQueryByTaskPriorityAsPost()
	  {
		int taskPriority = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PRIORITY;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskPriority"] = taskPriority;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskPriority(taskPriority);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByAssigned()
	  public virtual void testQueryByAssigned()
	  {
		given().queryParam("assigned", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssigned();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByAssignedAsPost()
	  public virtual void testQueryByAssignedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["assigned"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskAssigned();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByWithCandidateGroups()
	  public virtual void testQueryByWithCandidateGroups()
	  {
		given().queryParam("withCandidateGroups", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByWithCandidateGroupsAsPost()
	  public virtual void testQueryByWithCandidateGroupsAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["withCandidateGroups"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByWithoutCandidateGroups()
	  public virtual void testQueryByWithoutCandidateGroups()
	  {
		given().queryParam("withoutCandidateGroups", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withoutCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByWithoutCandidateGroupsAsPost()
	  public virtual void testQueryByWithoutCandidateGroupsAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["withoutCandidateGroups"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).withoutCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnassigned()
	  public virtual void testQueryByUnassigned()
	  {
		given().queryParam("unassigned", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskUnassigned();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnassignedAsPost()
	  public virtual void testQueryByUnassignedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["unassigned"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskUnassigned();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinished()
	  public virtual void testQueryByFinished()
	  {
		given().queryParam("finished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).finished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinishedAsPost()
	  public virtual void testQueryByFinishedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["finished"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).finished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnfinished()
	  public virtual void testQueryByUnfinished()
	  {
		given().queryParam("unfinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).unfinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUnfinishedAsPost()
	  public virtual void testQueryByUnfinishedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["unfinished"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).unfinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessFinished()
	  public virtual void testQueryByProcessFinished()
	  {
		given().queryParam("processFinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processFinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessFinishedAsPost()
	  public virtual void testQueryByProcessFinishedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processFinished"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processFinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessUnfinished()
	  public virtual void testQueryByProcessUnfinished()
	  {
		given().queryParam("processUnfinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processUnfinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessUnfinishedAsPost()
	  public virtual void testQueryByProcessUnfinishedAsPost()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processUnfinished"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processUnfinished();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskParentTaskId()
	  public virtual void testQueryByTaskParentTaskId()
	  {
		string taskParentTaskId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PARENT_TASK_ID;

		given().queryParam("taskParentTaskId", taskParentTaskId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskParentTaskId(taskParentTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskParentTaskIdAsPost()
	  public virtual void testQueryByTaskParentTaskIdAsPost()
	  {
		string taskParentTaskId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_PARENT_TASK_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskParentTaskId"] = taskParentTaskId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskParentTaskId(taskParentTaskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDate()
	  public virtual void testQueryByTaskDueDate()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		given().queryParam("taskDueDate", due).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueDate(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDateAsPost()
	  public virtual void testQueryByTaskDueDateAsPost()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDueDate"] = DateTimeUtil.parseDate(due);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueDate(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDateBefore()
	  public virtual void testQueryByTaskDueDateBefore()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		given().queryParam("taskDueDateBefore", due).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueBefore(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDateBeforeAsPost()
	  public virtual void testQueryByTaskDueDateBeforeAsPost()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDueDateBefore"] = DateTimeUtil.parseDate(due);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueBefore(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDateAfter()
	  public virtual void testQueryByTaskDueDateAfter()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		given().queryParam("taskDueDateAfter", due).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueAfter(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDueDateAfterAsPost()
	  public virtual void testQueryByTaskDueDateAfterAsPost()
	  {
		string due = MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskDueDateAfter"] = DateTimeUtil.parseDate(due);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDueAfter(DateTimeUtil.parseDate(due));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDate()
	  public virtual void testQueryByTaskFollowUpDate()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		given().queryParam("taskFollowUpDate", followUp).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpDate(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDateAsPost()
	  public virtual void testQueryByTaskFollowUpDateAsPost()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskFollowUpDate"] = DateTimeUtil.parseDate(followUp);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpDate(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDateBefore()
	  public virtual void testQueryByTaskFollowUpDateBefore()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		given().queryParam("taskFollowUpDateBefore", followUp).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpBefore(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDateBeforeAsPost()
	  public virtual void testQueryByTaskFollowUpDateBeforeAsPost()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskFollowUpDateBefore"] = DateTimeUtil.parseDate(followUp);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpBefore(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDateAfter()
	  public virtual void testQueryByTaskFollowUpDateAfter()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		given().queryParam("taskFollowUpDateAfter", followUp).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpAfter(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskFollowUpDateAfterAsPost()
	  public virtual void testQueryByTaskFollowUpDateAfterAsPost()
	  {
		string followUp = MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskFollowUpDateAfter"] = DateTimeUtil.parseDate(followUp);

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskFollowUpAfter(DateTimeUtil.parseDate(followUp));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByStartedBefore()
		public virtual void testQueryByStartedBefore()
		{
		  string startedBefore = MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME;

		  given().queryParam("startedBefore", startedBefore).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).startedBefore(DateTimeUtil.parseDate(startedBefore));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByStartedBeforeAsPost()
		public virtual void testQueryByStartedBeforeAsPost()
		{
		  string startedBefore = MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME;

		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["startedBefore"] = DateTimeUtil.parseDate(startedBefore);

		  given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).startedBefore(DateTimeUtil.parseDate(startedBefore));
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByStartedAfter()
		public virtual void testQueryByStartedAfter()
		{
		  string startedAfter = MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME;

		  given().queryParam("startedAfter", startedAfter).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).startedAfter(DateTimeUtil.parseDate(startedAfter));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByStartedAfterAsPost()
		public virtual void testQueryByStartedAfterAsPost()
		{
		  string startedAfter = MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME;

		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["startedAfter"] = DateTimeUtil.parseDate(startedAfter);

		  given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).startedAfter(DateTimeUtil.parseDate(startedAfter));
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinishedBefore()
		public virtual void testQueryByFinishedBefore()
		{
		  string finishedBefore = MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME;

		  given().queryParam("finishedBefore", finishedBefore).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).finishedBefore(DateTimeUtil.parseDate(finishedBefore));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinishedBeforeAsPost()
		public virtual void testQueryByFinishedBeforeAsPost()
		{
		  string finishedBefore = MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME;

		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["finishedBefore"] = DateTimeUtil.parseDate(finishedBefore);

		  given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).finishedBefore(DateTimeUtil.parseDate(finishedBefore));
		}


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinishedAfter()
		public virtual void testQueryByFinishedAfter()
		{
		  string finishedAfter = MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME;

		  given().queryParam("finishedAfter", finishedAfter).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).finishedAfter(DateTimeUtil.parseDate(finishedAfter));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByFinishedAfterAsPost()
		public virtual void testQueryByFinishedAfterAsPost()
		{
		  string finishedAfter = MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME;

		  IDictionary<string, object> @params = new Dictionary<string, object>();
		  @params["finishedAfter"] = DateTimeUtil.parseDate(finishedAfter);

		  given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		  verify(mockedQuery).finishedAfter(DateTimeUtil.parseDate(finishedAfter));
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskVariable()
	  public virtual void testQueryByTaskVariable()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string variableParameter = variableName + "_eq_" + variableValue;

		string queryValue = variableParameter;

		given().queryParam("taskVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskVariableValueEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskVariableAsPost()
	  public virtual void testQueryByTaskVariableAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskVariableValueEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidTaskVariable()
	  public virtual void testQueryByInvalidTaskVariable()
	  {
		// invalid comparator
		string invalidComparator = "anInvalidComparator";
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_" + invalidComparator + "_" + variableValue;

		given().queryParam("taskVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";

		given().queryParam("taskVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidTaskVariableAsPost()
	  public virtual void testQueryByInvalidTaskVariableAsPost()
	  {
		// invalid comparator
		string invalidComparator = "anInvalidComparator";
		string variableName = "varName";
		string variableValue = "varValue";

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = invalidComparator;
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["taskVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid variable comparator specified: " + invalidComparator)).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessVariable()
	  public virtual void testQueryByProcessVariable()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string variableParameter = variableName + "_eq_" + variableValue;

		string queryValue = variableParameter;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueEquals(variableName, variableValue);

		// greater then
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueGreaterThan(variableName, variableValue);

		// greater then equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueGreaterThanOrEquals(variableName, variableValue);

		// lower then
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueLessThan(variableName, variableValue);

		// lower then equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueLessThanOrEquals(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessVariableAsPost()
	  public virtual void testQueryByProcessVariableAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).processVariableValueEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessVariable()
	  public virtual void testQueryByInvalidProcessVariable()
	  {
		// invalid comparator
		string invalidComparator = "anInvalidComparator";
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_" + invalidComparator + "_" + variableValue;

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid process variable comparator specified: " + invalidComparator)).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		// invalid format
		queryValue = "invalidFormattedVariableQuery";

		given().queryParam("processVariables", queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("variable query parameter has to have format KEY_OPERATOR_VALUE")).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByInvalidProcessVariableAsPost()
	  public virtual void testQueryByInvalidProcessVariableAsPost()
	  {
		// invalid comparator
		string invalidComparator = "anInvalidComparator";
		string variableName = "varName";
		string variableValue = "varValue";

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = invalidComparator;
		variableJson["value"] = variableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["processVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Invalid process variable comparator specified: " + invalidComparator)).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionId()
	  public virtual void testQueryByCaseDefinitionId()
	  {
		string caseDefinitionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_ID;

		given().queryParam("caseDefinitionId", caseDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionId(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionIdAsPost()
	  public virtual void testQueryByCaseDefinitionIdAsPost()
	  {
		string caseDefinitionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["caseDefinitionId"] = caseDefinitionId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionId(caseDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionKey()
	  public virtual void testQueryByCaseDefinitionKey()
	  {
		string caseDefinitionKey = "aCaseDefKey";

		given().queryParam("caseDefinitionKey", caseDefinitionKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionKey(caseDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionKeyAsPost()
	  public virtual void testQueryByCaseDefinitionKeyAsPost()
	  {
		string caseDefinitionKey = "aCaseDefKey";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["caseDefinitionKey"] = caseDefinitionKey;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionKey(caseDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionName()
	  public virtual void testQueryByCaseDefinitionName()
	  {
		string caseDefinitionName = "aCaseDefName";

		given().queryParam("caseDefinitionName", caseDefinitionName).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionName(caseDefinitionName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseDefinitionNameAsPost()
	  public virtual void testQueryByCaseDefinitionNameAsPost()
	  {
		string caseDefinitionName = "aCaseDefName";

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["caseDefinitionName"] = caseDefinitionName;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseDefinitionName(caseDefinitionName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseInstanceId()
	  public virtual void testQueryByCaseInstanceId()
	  {
		string caseInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_INST_ID;

		given().queryParam("caseInstanceId", caseInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseInstanceIdAsPost()
	  public virtual void testQueryByCaseInstanceIdAsPost()
	  {
		string caseInstanceId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_INST_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["caseInstanceId"] = caseInstanceId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseInstanceId(caseInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseExecutionId()
	  public virtual void testQueryByCaseExecutionId()
	  {
		string caseExecutionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_EXEC_ID;

		given().queryParam("caseExecutionId", caseExecutionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseExecutionId(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCaseExecutionIdAsPost()
	  public virtual void testQueryByCaseExecutionIdAsPost()
	  {
		string caseExecutionId = MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_EXEC_ID;

		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["caseExecutionId"] = caseExecutionId;

		given().contentType(POST_JSON_CONTENT_TYPE).body(@params).then().expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseExecutionId(caseExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricTaskInstanceQuery(createMockHistoricTaskInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

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
		mockedQuery = setUpMockHistoricTaskInstanceQuery(createMockHistoricTaskInstancesTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

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
//ORIGINAL LINE: @Test public void testQueryTaskInvolvedUser()
	  public virtual void testQueryTaskInvolvedUser()
	  {
		string taskInvolvedUser = MockProvider.EXAMPLE_HISTORIC_TASK_INST_TASK_INVOLVED_USER;
		given().queryParam("taskInvolvedUser", taskInvolvedUser).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskInvolvedUser(taskInvolvedUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryTaskInvolvedGroup()
	  public virtual void testQueryTaskInvolvedGroup()
	  {
		string taskInvolvedGroup = MockProvider.EXAMPLE_HISTORIC_TASK_INST_TASK_INVOLVED_GROUP;
		given().queryParam("taskInvolvedGroup", taskInvolvedGroup).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskInvolvedGroup(taskInvolvedGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryTaskHadCandidateUser()
	  public virtual void testQueryTaskHadCandidateUser()
	  {
		string taskHadCandidateUser = MockProvider.EXAMPLE_HISTORIC_TASK_INST_TASK_HAD_CANDIDATE_USER;
		given().queryParam("taskHadCandidateUser", taskHadCandidateUser).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskHadCandidateUser(taskHadCandidateUser);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryTaskHadCandidateGroup()
	  public virtual void testQueryTaskHadCandidateGroup()
	  {
		string taskHadCandidateGroup = MockProvider.EXAMPLE_HISTORIC_TASK_INST_TASK_HAD_CANDIDATE_GROUP;
		given().queryParam("taskHadCandidateGroup", taskHadCandidateGroup).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskHadCandidateGroup(taskHadCandidateGroup);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDefinitionKeyIn()
	  public virtual void testQueryByTaskDefinitionKeyIn()
	  {

		string taskDefinitionKey1 = "aTaskDefinitionKey";
		string taskDefinitionKey2 = "anotherTaskDefinitionKey";

		given().queryParam("taskDefinitionKeyIn", taskDefinitionKey1 + "," + taskDefinitionKey2).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDefinitionKeyIn(taskDefinitionKey1, taskDefinitionKey2);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskDefinitionKeyInAsPost()
	  public virtual void testQueryByTaskDefinitionKeyInAsPost()
	  {

		string taskDefinitionKey1 = "aTaskDefinitionKey";
		string taskDefinitionKey2 = "anotherTaskDefinitionKey";

		IList<string> taskDefinitionKeys = new List<string>();
		taskDefinitionKeys.Add(taskDefinitionKey1);
		taskDefinitionKeys.Add(taskDefinitionKey2);

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["taskDefinitionKeyIn"] = taskDefinitionKeys;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(HISTORIC_TASK_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).taskDefinitionKeyIn(taskDefinitionKey1, taskDefinitionKey2);
		verify(mockedQuery).list();
	  }


	  private IList<HistoricTaskInstance> createMockHistoricTaskInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricTaskInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricTaskInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

	  protected internal virtual void verifyHistoricTaskInstanceEntries(string content)
	  {
		string returnedId = from(content).getString("[0].id");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedActivityInstanceId = from(content).getString("[0].activityInstanceId");
		string returnedName = from(content).getString("[0].name");
		string returnedDescription = from(content).getString("[0].description");
		string returnedDeleteReason = from(content).getString("[0].deleteReason");
		string returnedOwner = from(content).getString("[0].owner");
		string returnedAssignee = from(content).getString("[0].assignee");
		DateTime returnedStartTime = DateTimeUtil.parseDate(from(content).getString("[0].startTime"));
		DateTime returnedEndTime = DateTimeUtil.parseDate(from(content).getString("[0].endTime"));
		long? returnedDurationInMillis = from(content).getLong("[0].duration");
		string returnedTaskDefinitionKey = from(content).getString("[0].taskDefinitionKey");
		int returnedPriority = from(content).getInt("[0].priority");
		string returnedParentTaskId = from(content).getString("[0].parentTaskId");
		DateTime returnedDue = DateTimeUtil.parseDate(from(content).getString("[0].due"));
		DateTime returnedFollow = DateTimeUtil.parseDate(from(content).getString("[0].followUp"));
		string returnedCaseDefinitionKey = from(content).getString("[0].caseDefinitionKey");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedCaseExecutionId = from(content).getString("[0].caseExecutionId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		DateTime returnedRemovalTime = DateTimeUtil.parseDate(from(content).getString("[0].removalTime"));
		string returnedRootProcessInstanceId = from(content).getString("[0].rootProcessInstanceId");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_INST_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_ACT_INST_ID, returnedActivityInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_EXEC_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_PROC_DEF_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_NAME, returnedName);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_DESCRIPTION, returnedDescription);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_DELETE_REASON, returnedDeleteReason);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_OWNER, returnedOwner);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_ASSIGNEE, returnedAssignee);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_TASK_INST_START_TIME), returnedStartTime);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_TASK_INST_END_TIME), returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_DURATION, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_DEF_KEY, returnedTaskDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_PRIORITY, returnedPriority);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_TASK_INST_DUE_DATE), returnedDue);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_TASK_INST_FOLLOW_UP_DATE), returnedFollow);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_PARENT_TASK_ID, returnedParentTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_KEY, returnedCaseDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_DEF_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_INST_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_CASE_EXEC_ID, returnedCaseExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_TASK_INST_REMOVAL_TIME), returnedRemovalTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_TASK_INST_ROOT_PROC_INST_ID, returnedRootProcessInstanceId);
	  }

	}

}