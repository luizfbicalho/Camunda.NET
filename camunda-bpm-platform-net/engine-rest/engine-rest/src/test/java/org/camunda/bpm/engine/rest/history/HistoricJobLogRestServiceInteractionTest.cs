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
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class HistoricJobLogRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_JOB_LOG_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/job-log";
	  protected internal static readonly string SINGLE_HISTORIC_JOB_LOG_RESOURCE_URL = HISTORIC_JOB_LOG_RESOURCE_URL + "/{id}";
	  protected internal static readonly string HISTORIC_JOB_LOG_RESOURCE_GET_STACKTRACE_URL = SINGLE_HISTORIC_JOB_LOG_RESOURCE_URL + "/stacktrace";

	  protected internal ProcessEngine namedProcessEngine;
	  protected internal HistoryService mockHistoryService;

	  protected internal HistoricJobLogQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = mock(typeof(HistoricJobLogQuery));

		HistoricJobLog mockedHistoricJobLog = MockProvider.createMockHistoricJobLog();

		when(mockQuery.singleResult()).thenReturn(mockedHistoricJobLog);
		when(mockQuery.logId(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID)).thenReturn(mockQuery);

		mockHistoryService = mock(typeof(HistoryService));
		when(mockHistoryService.createHistoricJobLogQuery()).thenReturn(mockQuery);

		namedProcessEngine = getProcessEngine(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
		when(namedProcessEngine.HistoryService).thenReturn(mockHistoryService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricJobLogGet()
	  public virtual void testSimpleHistoricJobLogGet()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID)).body("timestamp", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_TIMESTAMP)).body("removalTime", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_REMOVAL_TIME)).body("jobId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_ID)).body("jobDueDate", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_DUE_DATE)).body("jobRetries", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_RETRIES)).body("jobPriority", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_PRIORITY)).body("jobExceptionMessage", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_EXCEPTION_MSG)).body("jobDefinitionId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_ID)).body("jobDefinitionType", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_TYPE)).body("jobDefinitionConfiguration", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_JOB_DEF_CONFIG)).body("activityId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ACTIVITY_ID)).body("executionId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_EXECUTION_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_PROC_INST_ID)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_ID)).body("processDefinitionKey", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_PROC_DEF_KEY)).body("deploymentId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_DEPLOYMENT_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("rootProcessInstanceId", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ROOT_PROC_INST_ID)).body("creationLog", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_IS_CREATION_LOG)).body("failureLog", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_IS_FAILURE_LOG)).body("successLog", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_IS_SUCCESS_LOG)).body("deletionLog", equalTo(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_IS_DELETION_LOG)).when().get(SINGLE_HISTORIC_JOB_LOG_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).logId(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID);
		inOrder.verify(mockQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricJobLogGetIdDoesntExist()
	  public virtual void testHistoricJobLogGetIdDoesntExist()
	  {
		string id = "nonExistingId";

		HistoricJobLogQuery invalidQueryNonExistingHistoricJobLog = mock(typeof(HistoricJobLogQuery));
		when(mockHistoryService.createHistoricJobLogQuery().logId(id)).thenReturn(invalidQueryNonExistingHistoricJobLog);
		when(invalidQueryNonExistingHistoricJobLog.singleResult()).thenReturn(null);

		given().pathParam("id", id).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic job log with id " + id + " does not exist")).when().get(SINGLE_HISTORIC_JOB_LOG_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktrace()
	  public virtual void testGetStacktrace()
	  {
		string stacktrace = "aStacktrace";
		when(mockHistoryService.getHistoricJobLogExceptionStacktrace(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID)).thenReturn(stacktrace);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).when().get(HISTORIC_JOB_LOG_RESOURCE_GET_STACKTRACE_URL);

		string content = response.asString();
		Assert.assertEquals(stacktrace, content);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktraceJobNotFound()
	  public virtual void testGetStacktraceJobNotFound()
	  {
		string exceptionMessage = "historic job log not found";
		doThrow(new ProcessEngineException(exceptionMessage)).when(mockHistoryService).getHistoricJobLogExceptionStacktrace(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(exceptionMessage)).when().get(HISTORIC_JOB_LOG_RESOURCE_GET_STACKTRACE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktraceThrowsAuthorizationException()
	  public virtual void testGetStacktraceThrowsAuthorizationException()
	  {
		string exceptionMessage = "expected exception";
		doThrow(new AuthorizationException(exceptionMessage)).when(mockHistoryService).getHistoricJobLogExceptionStacktrace(MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID);

		given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_JOB_LOG_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(exceptionMessage)).when().get(HISTORIC_JOB_LOG_RESOURCE_GET_STACKTRACE_URL);
	  }
	}

}