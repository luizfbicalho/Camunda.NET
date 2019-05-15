﻿/*
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
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;

	public class HistoricExternalTaskLogRestServiceInteractionTest : AbstractRestServiceTest
	{

	  protected internal static readonly string HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/external-task-log";
	  protected internal static readonly string SINGLE_HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL = HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL + "/{id}";
	  protected internal static readonly string HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_GET_ERROR_DETAILS_URL = SINGLE_HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL + "/error-details";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();
	  protected internal ProcessEngine namedProcessEngine;
	  protected internal HistoryService mockHistoryService;

	  protected internal HistoricExternalTaskLogQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = mock(typeof(HistoricExternalTaskLogQuery));

		HistoricExternalTaskLog mockedHistoricExternalTaskLog = MockProvider.createMockHistoricExternalTaskLog();

		when(mockQuery.singleResult()).thenReturn(mockedHistoricExternalTaskLog);
		when(mockQuery.logId(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID)).thenReturn(mockQuery);

		mockHistoryService = mock(typeof(HistoryService));
		when(mockHistoryService.createHistoricExternalTaskLogQuery()).thenReturn(mockQuery);

		namedProcessEngine = getProcessEngine(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
		when(namedProcessEngine.HistoryService).thenReturn(mockHistoryService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricExternalTaskLogGet()
	  public virtual void testSimpleHistoricExternalTaskLogGet()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID)).body("timestamp", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TIMESTAMP)).body("removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_REMOVAL_TIME)).body("externalTaskId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXTERNAL_TASK_ID)).body("topicName", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_TOPIC_NAME)).body("workerId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_WORKER_ID)).body("retries", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_RETRIES)).body("priority", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PRIORITY)).body("errorMessage", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ERROR_MSG)).body("activityId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_ID)).body("activityInstanceId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ACTIVITY_INSTANCE_ID)).body("executionId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_EXECUTION_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_INST_ID)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_ID)).body("processDefinitionKey", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_PROC_DEF_KEY)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("rootProcessInstanceId", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ROOT_PROC_INST_ID)).body("creationLog", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_CREATION_LOG)).body("failureLog", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_FAILURE_LOG)).body("successLog", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_SUCCESS_LOG)).body("deletionLog", equalTo(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_IS_DELETION_LOG)).when().get(SINGLE_HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).logId(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID);
		inOrder.verify(mockQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricExternalTaskLogGetIdDoesntExist()
	  public virtual void testHistoricExternalTaskLogGetIdDoesntExist()
	  {
		string id = "nonExistingId";

		HistoricExternalTaskLogQuery invalidQueryNonExistingHistoricExternalTaskLog = mock(typeof(HistoricExternalTaskLogQuery));
		when(mockHistoryService.createHistoricExternalTaskLogQuery().logId(id)).thenReturn(invalidQueryNonExistingHistoricExternalTaskLog);
		when(invalidQueryNonExistingHistoricExternalTaskLog.singleResult()).thenReturn(null);

		given().pathParam("id", id).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic external task log with id " + id + " does not exist")).when().get(SINGLE_HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetails()
	  public virtual void testGetErrorDetails()
	  {
		string errorDetails = "someErrorDetails";
		when(mockHistoryService.getHistoricExternalTaskLogErrorDetails(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID)).thenReturn(errorDetails);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_GET_ERROR_DETAILS_URL);

		string content = response.asString();
		assertEquals(errorDetails, content);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsExternalTaskNotFound()
	  public virtual void testGetErrorDetailsExternalTaskNotFound()
	  {
		string exceptionMessage = "historic external task log not found";
		doThrow(new ProcessEngineException(exceptionMessage)).when(mockHistoryService).getHistoricExternalTaskLogErrorDetails(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(exceptionMessage)).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_GET_ERROR_DETAILS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetErrorDetailsThrowsAuthorizationException()
	  public virtual void testGetErrorDetailsThrowsAuthorizationException()
	  {
		string exceptionMessage = "expected exception";
		doThrow(new AuthorizationException(exceptionMessage)).when(mockHistoryService).getHistoricExternalTaskLogErrorDetails(MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_EXTERNAL_TASK_LOG_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(exceptionMessage)).when().get(HISTORIC_EXTERNAL_TASK_LOG_RESOURCE_GET_ERROR_DETAILS_URL);
	  }

	}

}