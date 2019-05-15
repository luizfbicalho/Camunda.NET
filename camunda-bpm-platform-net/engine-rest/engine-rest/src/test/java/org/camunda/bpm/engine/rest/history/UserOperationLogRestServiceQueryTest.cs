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
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CLAIM;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using UserOperationLogEntryDto = org.camunda.bpm.engine.rest.dto.history.UserOperationLogEntryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string USER_OPERATION_LOG_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/user-operation";

	  protected internal static readonly string USER_OPERATION_LOG_COUNT_RESOURCE_URL = USER_OPERATION_LOG_RESOURCE_URL + "/count";

	  protected internal UserOperationLogQuery queryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpMock()
	  public virtual void setUpMock()
	  {
		IList<UserOperationLogEntry> entries = MockProvider.createUserOperationLogEntries();
		queryMock = mock(typeof(UserOperationLogQuery));
		when(queryMock.list()).thenReturn(entries);
		when(queryMock.listPage(anyInt(), anyInt())).thenReturn(entries);
		when(queryMock.count()).thenReturn((long) entries.Count);
		when(processEngine.HistoryService.createUserOperationLogQuery()).thenReturn(queryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(USER_OPERATION_LOG_COUNT_RESOURCE_URL);

		verify(queryMock).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);

		verify(queryMock, never()).deploymentId(anyString());
		verify(queryMock, never()).processDefinitionId(anyString());
		verify(queryMock, never()).processDefinitionKey(anyString());
		verify(queryMock, never()).processInstanceId(anyString());
		verify(queryMock, never()).executionId(anyString());
		verify(queryMock, never()).caseDefinitionId(anyString());
		verify(queryMock, never()).caseInstanceId(anyString());
		verify(queryMock, never()).caseExecutionId(anyString());
		verify(queryMock, never()).taskId(anyString());
		verify(queryMock, never()).jobId(anyString());
		verify(queryMock, never()).jobDefinitionId(anyString());
		verify(queryMock, never()).batchId(anyString());
		verify(queryMock, never()).userId(anyString());
		verify(queryMock, never()).operationId(anyString());
		verify(queryMock, never()).externalTaskId(anyString());
		verify(queryMock, never()).operationType(anyString());
		verify(queryMock, never()).entityType(anyString());
		verify(queryMock, never()).category(anyString());
		verify(queryMock, never()).property(anyString());
		verify(queryMock, never()).afterTimestamp(any(typeof(DateTime)));
		verify(queryMock, never()).beforeTimestamp(any(typeof(DateTime)));
		verify(queryMock, never()).orderByTimestamp();
		verify(queryMock, never()).asc();
		verify(queryMock, never()).desc();
		verify(queryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryParameter()
	  public virtual void testQueryParameter()
	  {
		Response response = given().queryParam("deploymentId", "a-deployment-id").queryParam("processDefinitionId", "1").queryParam("processDefinitionKey", "6").queryParam("processInstanceId", "2").queryParam("executionId", "3").queryParam("caseDefinitionId", "x").queryParam("caseInstanceId", "y").queryParam("caseExecutionId", "z").queryParam("taskId", "4").queryParam("jobId", "7").queryParam("jobDefinitionId", "8").queryParam("batchId", MockProvider.EXAMPLE_BATCH_ID).queryParam("userId", "icke").queryParam("operationId", "5").queryParam("externalTaskId", "1").queryParam("operationType", OPERATION_TYPE_CLAIM).queryParam("entityType", EntityTypes.TASK).queryParam("entityTypeIn", EntityTypes.TASK + "," + EntityTypes.VARIABLE).queryParam("category", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).queryParam("categoryIn", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER + "," + org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).queryParam("property", "owner").then().expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);

		verify(queryMock).deploymentId("a-deployment-id");
		verify(queryMock).processDefinitionId("1");
		verify(queryMock).processDefinitionKey("6");
		verify(queryMock).processInstanceId("2");
		verify(queryMock).executionId("3");
		verify(queryMock).caseDefinitionId("x");
		verify(queryMock).caseInstanceId("y");
		verify(queryMock).caseExecutionId("z");
		verify(queryMock).taskId("4");
		verify(queryMock).jobId("7");
		verify(queryMock).jobDefinitionId("8");
		verify(queryMock).batchId(MockProvider.EXAMPLE_BATCH_ID);
		verify(queryMock).userId("icke");
		verify(queryMock).operationId("5");
		verify(queryMock).externalTaskId("1");
		verify(queryMock).operationType(OPERATION_TYPE_CLAIM);
		verify(queryMock).entityType(EntityTypes.TASK);
		verify(queryMock).entityTypeIn(EntityTypes.TASK, EntityTypes.VARIABLE);
		verify(queryMock).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
		verify(queryMock).categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
		verify(queryMock).property("owner");
		verify(queryMock).list();

		string json = response.asString();
		UserOperationLogEntryDto actual = from(json).getObject("[0]", typeof(UserOperationLogEntryDto));
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_LOG_ID, actual.Id);
		assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, actual.DeploymentId);
		assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, actual.ProcessDefinitionId);
		assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, actual.ProcessDefinitionKey);
		assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, actual.ProcessInstanceId);
		assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, actual.ExecutionId);
		assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, actual.CaseDefinitionId);
		assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, actual.CaseInstanceId);
		assertEquals(MockProvider.EXAMPLE_CASE_EXECUTION_ID, actual.CaseExecutionId);
		assertEquals(MockProvider.EXAMPLE_TASK_ID, actual.TaskId);
		assertEquals(MockProvider.EXAMPLE_JOB_ID, actual.JobId);
		assertEquals(MockProvider.EXAMPLE_JOB_DEFINITION_ID, actual.JobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_BATCH_ID, actual.BatchId);
		assertEquals(MockProvider.EXAMPLE_USER_ID, actual.UserId);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_TIMESTAMP, from(json).getString("[0].timestamp"));
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_ID, actual.OperationId);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_TYPE, actual.OperationType);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_ENTITY, actual.EntityType);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_PROPERTY, actual.Property);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_ORG_VALUE, actual.OrgValue);
		assertEquals(MockProvider.EXAMPLE_USER_OPERATION_NEW_VALUE, actual.NewValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryAfterTimestamp()
	  public virtual void testQueryAfterTimestamp()
	  {
		given().queryParam("afterTimestamp", MockProvider.EXAMPLE_USER_OPERATION_TIMESTAMP).expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).afterTimestamp(DateTimeUtil.parseDate(MockProvider.EXAMPLE_USER_OPERATION_TIMESTAMP));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryBeforeTimestamp()
	  public virtual void testQueryBeforeTimestamp()
	  {
		given().queryParam("beforeTimestamp", MockProvider.EXAMPLE_USER_OPERATION_TIMESTAMP).expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).beforeTimestamp(DateTimeUtil.parseDate(MockProvider.EXAMPLE_USER_OPERATION_TIMESTAMP));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByTimestampAscending()
	  public virtual void testSortByTimestampAscending()
	  {
		given().queryParam("sortBy", "timestamp").queryParam("sortOrder", "asc").expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).orderByTimestamp();
		verify(queryMock).asc();
		verify(queryMock, never()).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByTimestampDescending()
	  public virtual void testSortByTimestampDescending()
	  {
		given().queryParam("sortBy", "timestamp").queryParam("sortOrder", "desc").expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).orderByTimestamp();
		verify(queryMock).desc();
		verify(queryMock, never()).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSortByParameter()
	  public virtual void testInvalidSortByParameter()
	  {
		given().queryParam("sortBy", "unknownField").queryParam("sortOrder", "desc").expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).when().get(USER_OPERATION_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testPagination()
	  public virtual void testPagination()
	  {
		given().queryParam("firstResult", 7).queryParam("maxResults", 13).expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).listPage(7, 13);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFirstResultMissing()
	  public virtual void testFirstResultMissing()
	  {
		given().queryParam("maxResults", 13).expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).listPage(0, 13);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMaxResultsMissing()
	  public virtual void testMaxResultsMissing()
	  {
		given().queryParam("firstResult", 7).expect().statusCode(Status.OK.StatusCode).when().get(USER_OPERATION_LOG_RESOURCE_URL);
		verify(queryMock).listPage(7, int.MaxValue);
	  }

	}

}