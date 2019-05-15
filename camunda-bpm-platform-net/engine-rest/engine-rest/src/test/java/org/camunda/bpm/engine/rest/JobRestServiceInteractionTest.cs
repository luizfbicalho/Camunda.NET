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
namespace org.camunda.bpm.engine.rest
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using Batch = org.camunda.bpm.engine.batch.Batch;
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using UpdateJobSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateSelectBuilder;
	using UpdateJobSuspensionStateTenantBuilder = org.camunda.bpm.engine.management.UpdateJobSuspensionStateTenantBuilder;
	using BatchDto = org.camunda.bpm.engine.rest.dto.batch.BatchDto;
	using HistoricProcessInstanceQueryDto = org.camunda.bpm.engine.rest.dto.history.HistoricProcessInstanceQueryDto;
	using JobSuspensionStateDto = org.camunda.bpm.engine.rest.dto.runtime.JobSuspensionStateDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MockJobBuilder = org.camunda.bpm.engine.rest.helper.MockJobBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using JsonPathUtil = org.camunda.bpm.engine.rest.util.JsonPathUtil;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class JobRestServiceInteractionTest : AbstractRestServiceTest
	{

	  private const string RETRIES = "retries";
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string JOB_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/job";
	  protected internal static readonly string SINGLE_JOB_RESOURCE_URL = JOB_RESOURCE_URL + "/{id}";
	  protected internal static readonly string JOB_RESOURCE_SET_RETRIES_URL = SINGLE_JOB_RESOURCE_URL + "/retries";
	  protected internal static readonly string JOBS_SET_RETRIES_URL = JOB_RESOURCE_URL + "/retries";
	  protected internal static readonly string JOB_RESOURCE_SET_PRIORITY_URL = SINGLE_JOB_RESOURCE_URL + "/priority";
	  protected internal static readonly string JOB_RESOURCE_EXECUTE_JOB_URL = SINGLE_JOB_RESOURCE_URL + "/execute";
	  protected internal static readonly string JOB_RESOURCE_GET_STACKTRACE_URL = SINGLE_JOB_RESOURCE_URL + "/stacktrace";
	  protected internal static readonly string JOB_RESOURCE_SET_DUEDATE_URL = SINGLE_JOB_RESOURCE_URL + "/duedate";
	  protected internal static readonly string JOB_RESOURCE_RECALC_DUEDATE_URL = JOB_RESOURCE_SET_DUEDATE_URL + "/recalculate";
	  protected internal static readonly string SINGLE_JOB_SUSPENDED_URL = SINGLE_JOB_RESOURCE_URL + "/suspended";
	  protected internal static readonly string JOB_SUSPENDED_URL = JOB_RESOURCE_URL + "/suspended";

	  private ProcessEngine namedProcessEngine;
	  private ManagementService mockManagementService;

	  private UpdateJobSuspensionStateTenantBuilder mockSuspensionStateBuilder;
	  private UpdateJobSuspensionStateSelectBuilder mockSuspensionStateSelectBuilder;

	  private JobQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {

		mockQuery = mock(typeof(JobQuery));
		Job mockedJob = (new MockJobBuilder()).id(MockProvider.EXAMPLE_JOB_ID).processInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID).executionId(MockProvider.EXAMPLE_EXECUTION_ID).retries(MockProvider.EXAMPLE_JOB_RETRIES).exceptionMessage(MockProvider.EXAMPLE_JOB_NO_EXCEPTION_MESSAGE).dueDate(DateTime.Now).priority(MockProvider.EXAMPLE_JOB_PRIORITY).jobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID).tenantId(MockProvider.EXAMPLE_TENANT_ID).createTime(DateTimeUtil.parseDate(MockProvider.EXAMPLE_JOB_CREATE_TIME)).build();

		when(mockQuery.singleResult()).thenReturn(mockedJob);
		when(mockQuery.jobId(MockProvider.EXAMPLE_JOB_ID)).thenReturn(mockQuery);

		mockManagementService = mock(typeof(ManagementService));
		when(mockManagementService.createJobQuery()).thenReturn(mockQuery);

		mockSuspensionStateSelectBuilder = mock(typeof(UpdateJobSuspensionStateSelectBuilder));
		when(mockManagementService.updateJobSuspensionState()).thenReturn(mockSuspensionStateSelectBuilder);

		mockSuspensionStateBuilder = mock(typeof(UpdateJobSuspensionStateTenantBuilder));
		when(mockSuspensionStateSelectBuilder.byJobId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byJobDefinitionId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byProcessInstanceId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byProcessDefinitionId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byProcessDefinitionKey(anyString())).thenReturn(mockSuspensionStateBuilder);

		namedProcessEngine = getProcessEngine(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
		when(namedProcessEngine.ManagementService).thenReturn(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetries()
	  public virtual void testSetJobRetries()
	  {
		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_JOB_RETRIES;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_RESOURCE_SET_RETRIES_URL);

		verify(mockManagementService).setJobRetries(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesNonExistentJob()
	  public virtual void testSetJobRetriesNonExistentJob()
	  {
		string expectedMessage = "No job found with id '" + MockProvider.NON_EXISTING_JOB_ID + "'.";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setJobRetries(MockProvider.NON_EXISTING_JOB_ID, MockProvider.EXAMPLE_JOB_RETRIES);

		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_JOB_RETRIES;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_RETRIES_URL);

		verify(mockManagementService).setJobRetries(MockProvider.NON_EXISTING_JOB_ID, MockProvider.EXAMPLE_JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesNegativeRetries()
	  public virtual void testSetJobRetriesNegativeRetries()
	  {

		string expectedMessage = "The number of job retries must be a non-negative Integer, but '" + MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES + "' has been provided.";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setJobRetries(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES);

		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_RETRIES_URL);

		verify(mockManagementService).setJobRetries(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesThrowsAuthorizationException()
	  public virtual void testSetJobRetriesThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockManagementService).setJobRetries(anyString(), anyInt());

		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_NEGATIVE_JOB_RETRIES;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(JOB_RESOURCE_SET_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleJobGet()
	  public virtual void testSimpleJobGet()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_JOB_ID)).body("processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("exceptionMessage", equalTo(MockProvider.EXAMPLE_JOB_NO_EXCEPTION_MESSAGE)).body("priority", equalTo(MockProvider.EXAMPLE_JOB_PRIORITY)).body("jobDefinitionId", equalTo(MockProvider.EXAMPLE_JOB_DEFINITION_ID)).body("tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("createTime", equalTo(MockProvider.EXAMPLE_JOB_CREATE_TIME)).when().get(SINGLE_JOB_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).jobId(MockProvider.EXAMPLE_JOB_ID);
		inOrder.verify(mockQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobGetIdDoesntExist()
	  public virtual void testJobGetIdDoesntExist()
	  {
		JobQuery invalidQueryNonExistingJob;
		invalidQueryNonExistingJob = mock(typeof(JobQuery));
		when(mockManagementService.createJobQuery().jobId(MockProvider.NON_EXISTING_JOB_ID)).thenReturn(invalidQueryNonExistingJob);
		when(invalidQueryNonExistingJob.singleResult()).thenReturn(null);

		string jobId = MockProvider.NON_EXISTING_JOB_ID;

		given().pathParam("id", jobId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Job with id " + jobId + " does not exist")).when().get(SINGLE_JOB_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJob()
	  public virtual void testExecuteJob()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(JOB_RESOURCE_EXECUTE_JOB_URL);

		verify(mockManagementService).executeJob(MockProvider.EXAMPLE_JOB_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobIdDoesntExist()
	  public virtual void testExecuteJobIdDoesntExist()
	  {
		string jobId = MockProvider.NON_EXISTING_JOB_ID;

		string expectedMessage = "No job found with id '" + jobId + "'";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).executeJob(MockProvider.NON_EXISTING_JOB_ID);

		given().pathParam("id", jobId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().post(JOB_RESOURCE_EXECUTE_JOB_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobRuntimeException()
	  public virtual void testExecuteJobRuntimeException()
	  {
		string jobId = MockProvider.EXAMPLE_JOB_ID;

		doThrow(new Exception("Runtime exception")).when(mockManagementService).executeJob(jobId);

		given().pathParam("id", jobId).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo("Runtime exception")).when().post(JOB_RESOURCE_EXECUTE_JOB_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteJobThrowsAuthorizationException()
	  public virtual void testExecuteJobThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockManagementService).executeJob(anyString());

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().post(JOB_RESOURCE_EXECUTE_JOB_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktrace()
	  public virtual void testGetStacktrace()
	  {
		string stacktrace = "aStacktrace";
		when(mockManagementService.getJobExceptionStacktrace(MockProvider.EXAMPLE_JOB_ID)).thenReturn(stacktrace);

		Response response = given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.OK.StatusCode).contentType(ContentType.TEXT).when().get(JOB_RESOURCE_GET_STACKTRACE_URL);

		string content = response.asString();
		Assert.assertEquals(stacktrace, content);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktraceJobNotFound()
	  public virtual void testGetStacktraceJobNotFound()
	  {
		string exceptionMessage = "job not found";
		doThrow(new ProcessEngineException(exceptionMessage)).when(mockManagementService).getJobExceptionStacktrace(MockProvider.EXAMPLE_JOB_ID);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(exceptionMessage)).when().get(JOB_RESOURCE_GET_STACKTRACE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktraceJobThrowsAuthorizationException()
	  public virtual void testGetStacktraceJobThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockManagementService).getJobExceptionStacktrace(MockProvider.EXAMPLE_JOB_ID);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(JOB_RESOURCE_GET_STACKTRACE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDuedate()
	  public virtual void testSetJobDuedate()
	  {
		DateTime newDuedate = MockProvider.createMockDuedate();
		IDictionary<string, object> duedateVariableJson = new Dictionary<string, object>();
		duedateVariableJson["duedate"] = newDuedate;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(duedateVariableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_RESOURCE_SET_DUEDATE_URL);

		verify(mockManagementService).setJobDuedate(MockProvider.EXAMPLE_JOB_ID, newDuedate);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDuedateNull()
	  public virtual void testSetJobDuedateNull()
	  {
		IDictionary<string, object> duedateVariableJson = new Dictionary<string, object>();
		duedateVariableJson["duedate"] = null;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(duedateVariableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_RESOURCE_SET_DUEDATE_URL);

		verify(mockManagementService).setJobDuedate(MockProvider.EXAMPLE_JOB_ID, null);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDuedateNonExistentJob()
	  public virtual void testSetJobDuedateNonExistentJob()
	  {
		DateTime newDuedate = MockProvider.createMockDuedate();
		string expectedMessage = "No job found with id '" + MockProvider.NON_EXISTING_JOB_ID + "'.";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setJobDuedate(MockProvider.NON_EXISTING_JOB_ID, newDuedate);

		IDictionary<string, object> duedateVariableJson = new Dictionary<string, object>();
		duedateVariableJson["duedate"] = newDuedate;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_ID).contentType(ContentType.JSON).body(duedateVariableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_DUEDATE_URL);

		verify(mockManagementService).setJobDuedate(MockProvider.NON_EXISTING_JOB_ID, newDuedate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobDuedateThrowsAuthorizationException()
	  public virtual void testSetJobDuedateThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockManagementService).setJobDuedate(anyString(), any(typeof(DateTime)));

		DateTime newDuedate = MockProvider.createMockDuedate();
		IDictionary<string, object> duedateVariableJson = new Dictionary<string, object>();
		duedateVariableJson["duedate"] = newDuedate;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(duedateVariableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(JOB_RESOURCE_SET_DUEDATE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJob()
	  public virtual void testActivateJob()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = false;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobId(MockProvider.EXAMPLE_JOB_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsProcessEngineException()
	  public virtual void testActivateThrowsProcessEngineException()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowsAuthorizationException()
	  public virtual void testActivateThrowsAuthorizationException()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = false;

		string expectedMessage = "expectedMessage";

		doThrow(new AuthorizationException(expectedMessage)).when(mockSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJob()
	  public virtual void testSuspendJob()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = true;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobId(MockProvider.EXAMPLE_JOB_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendedThrowsProcessEngineException()
	  public virtual void testSuspendedThrowsProcessEngineException()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = true;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendWithMultipleByParameters()
	  public virtual void testSuspendWithMultipleByParameters()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "Only one of jobId, jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(SINGLE_JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsAuthorizationException()
	  public virtual void testSuspendThrowsAuthorizationException()
	  {
		JobSuspensionStateDto dto = new JobSuspensionStateDto();
		dto.Suspended = true;

		string expectedMessage = "expectedMessage";

		doThrow(new AuthorizationException(expectedMessage)).when(mockSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(dto).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionKey()
	  public virtual void testActivateJobByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionKeyWithException()
	  public virtual void testActivateJobByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionKeyThrowsAuthorizationException()
	  public virtual void testActivateJobByProcessDefinitionKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionKey()
	  public virtual void testSuspendJobByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionKeyWithException()
	  public virtual void testSuspendJobByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionKeyThrowsAuthorizationException()
	  public virtual void testSuspendJobByProcessDefinitionKeyThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionKeyAndTenantId()
	  public virtual void testActivateJobByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testActivateJobByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionKeyAndTenantId()
	  public virtual void testSuspendJobByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testSuspendJobByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionId()
	  public virtual void testActivateJobByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionIdWithException()
	  public virtual void testActivateJobByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessDefinitionIdThrowsAuthorizationException()
	  public virtual void testActivateJobByProcessDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionId()
	  public virtual void testSuspendJobByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionIdWithException()
	  public virtual void testSuspendJobByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessDefinitionIdThrowsAuthorizationException()
	  public virtual void testSuspendJobByProcessDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessInstanceId()
	  public virtual void testActivateJobByProcessInstanceId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessInstanceIdWithException()
	  public virtual void testActivateJobByProcessInstanceIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByProcessInstanceIdThrowsAuthorizationException()
	  public virtual void testActivateJobByProcessInstanceIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessInstanceId()
	  public virtual void testSuspendJobByProcessInstanceId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessInstanceId(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessInstanceIdWithException()
	  public virtual void testSuspendJobByProcessInstanceIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByProcessInstanceIdThrowsAuthorizationException()
	  public virtual void testSuspendJobByProcessInstanceIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByJobDefinitionId()
	  public virtual void testActivateJobByJobDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByJobDefinitionIdThrowsAuthorizationException()
	  public virtual void testActivateJobByJobDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByJobDefinitionId()
	  public virtual void testSuspendJobByJobDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByJobDefinitionIdThrowsAuthorizationException()
	  public virtual void testSuspendJobByJobDefinitionIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobByIdShouldThrowException()
	  public virtual void testActivateJobByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["jobId"] = MockProvider.EXAMPLE_JOB_ID;

		string message = "Either jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByIdShouldThrowException()
	  public virtual void testSuspendJobByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["jobId"] = MockProvider.EXAMPLE_JOB_ID;

		string message = "Either jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobByNothing()
	  public virtual void testSuspendJobByNothing()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "Either jobId, jobDefinitionId, processInstanceId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriority()
	  public virtual void testSetJobPriority()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_RESOURCE_SET_PRIORITY_URL);

		verify(mockManagementService).setJobPriority(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_JOB_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityToExtremeValue()
	  public virtual void testSetJobPriorityToExtremeValue()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = long.MaxValue;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_RESOURCE_SET_PRIORITY_URL);

		verify(mockManagementService).setJobPriority(MockProvider.EXAMPLE_JOB_ID, long.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityNonExistentJob()
	  public virtual void testSetJobPriorityNonExistentJob()
	  {
		string expectedMessage = "No job found with id '" + MockProvider.NON_EXISTING_JOB_ID + "'.";

		doThrow(new NotFoundException(expectedMessage)).when(mockManagementService).setJobPriority(MockProvider.NON_EXISTING_JOB_ID, MockProvider.EXAMPLE_JOB_PRIORITY);

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_PRIORITY;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_PRIORITY_URL);

		verify(mockManagementService).setJobPriority(MockProvider.NON_EXISTING_JOB_ID, MockProvider.EXAMPLE_JOB_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityFailure()
	  public virtual void testSetJobPriorityFailure()
	  {
		string expectedMessage = "No job found with id '" + MockProvider.EXAMPLE_JOB_ID + "'.";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setJobPriority(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_JOB_PRIORITY);

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_PRIORITY_URL);

		verify(mockManagementService).setJobPriority(MockProvider.EXAMPLE_JOB_ID, MockProvider.EXAMPLE_JOB_PRIORITY);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNullJobPriorityFailure()
	  public virtual void testSetNullJobPriorityFailure()
	  {
		string expectedMessage = "Priority for job '" + MockProvider.EXAMPLE_JOB_ID + "' cannot be null.";

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_RESOURCE_SET_PRIORITY_URL);

		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityThrowsAuthorizationException()
	  public virtual void testSetJobPriorityThrowsAuthorizationException()
	  {
		string message = "expected exception";
		doThrow(new AuthorizationException(message)).when(mockManagementService).setJobPriority(anyString(), anyInt());

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().put(JOB_RESOURCE_SET_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteJob()
	  public virtual void deleteJob()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_JOB_RESOURCE_URL);

		verify(mockManagementService).deleteJob(MockProvider.EXAMPLE_JOB_ID);
		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteNotExistingJob()
	  public virtual void deleteNotExistingJob()
	  {
		string jobId = MockProvider.NON_EXISTING_JOB_ID;

		string expectedMessage = "No job found with id '" + jobId + "'.";

		doThrow(new NullValueException(expectedMessage)).when(mockManagementService).deleteJob(jobId);

		given().pathParam("id", jobId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().delete(SINGLE_JOB_RESOURCE_URL);

		verify(mockManagementService).deleteJob(jobId);
		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteLockedJob()
	  public virtual void deleteLockedJob()
	  {
		string jobId = MockProvider.EXAMPLE_JOB_ID;

		string expectedMessage = "Cannot delete job when the job is being executed. Try again later.";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).deleteJob(jobId);

		given().pathParam("id", jobId).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo(expectedMessage)).when().delete(SINGLE_JOB_RESOURCE_URL);

		verify(mockManagementService).deleteJob(jobId);
		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void deleteJobThrowAuthorizationException()
	  public virtual void deleteJobThrowAuthorizationException()
	  {
		string jobId = MockProvider.EXAMPLE_JOB_ID;

		string expectedMessage = "Missing permissions";

		doThrow(new AuthorizationException(expectedMessage)).when(mockManagementService).deleteJob(jobId);

		given().pathParam("id", jobId).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().delete(SINGLE_JOB_RESOURCE_URL);

		verify(mockManagementService).deleteJob(jobId);
		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesAsync()
	  public virtual void testSetRetriesAsync()
	  {
		IList<string> ids = Arrays.asList(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(JobQuery)), anyInt())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["jobIds"] = ids;
		messageBodyJson[RETRIES] = 5;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_SET_RETRIES_URL);

		verifyBatchJson(response.asString());

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq(ids), eq((JobQuery) null), eq(5));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesAsyncWithQuery()
	  public virtual void testSetRetriesAsyncWithQuery()
	  {
		Batch batchEntity = MockProvider.createMockBatch();
		when(mockManagementService.setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(JobQuery)), anyInt())).thenReturn(batchEntity);

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = 5;
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		messageBodyJson["jobQuery"] = query;

		Response response = given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_SET_RETRIES_URL);

		verifyBatchJson(response.asString());

		verify(mockManagementService, times(1)).setJobRetriesAsync(eq((IList<string>) null), any(typeof(JobQuery)), Mockito.eq(5));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithBadRequestQuery()
	  public virtual void testSetRetriesWithBadRequestQuery()
	  {
		doThrow(new BadUserRequestException("job ids are empty")).when(mockManagementService).setJobRetriesAsync(eq((IList<string>) null), eq((JobQuery) null), anyInt());

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = 5;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(JOBS_SET_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithoutBody()
	  public virtual void testSetRetriesWithoutBody()
	  {
		given().contentType(ContentType.JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(JOBS_SET_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithNegativeRetries()
	  public virtual void testSetRetriesWithNegativeRetries()
	  {
		doThrow(new BadUserRequestException("retries are negative")).when(mockManagementService).setJobRetriesAsync(anyListOf(typeof(string)), any(typeof(JobQuery)), eq(-1));

		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson[RETRIES] = -1;
		HistoricProcessInstanceQueryDto query = new HistoricProcessInstanceQueryDto();
		messageBodyJson["jobQuery"] = query;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(JOBS_SET_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetRetriesWithoutRetries()
	  public virtual void testSetRetriesWithoutRetries()
	  {
		IDictionary<string, object> messageBodyJson = new Dictionary<string, object>();
		messageBodyJson["jobIds"] = null;

		given().contentType(ContentType.JSON).body(messageBodyJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(JOBS_SET_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRecalculateDuedateWithoutDateBase()
	  public virtual void testRecalculateDuedateWithoutDateBase()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(JOB_RESOURCE_RECALC_DUEDATE_URL);

		verify(mockManagementService).recalculateJobDuedate(MockProvider.EXAMPLE_JOB_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRecalculateDuedateCreationDateBased()
	  public virtual void testRecalculateDuedateCreationDateBased()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).queryParam("creationDateBased", true).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(JOB_RESOURCE_RECALC_DUEDATE_URL);

		verify(mockManagementService).recalculateJobDuedate(MockProvider.EXAMPLE_JOB_ID, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRecalculateDuedateCurrentDateBased()
	  public virtual void testRecalculateDuedateCurrentDateBased()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_ID).queryParam("creationDateBased", false).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().post(JOB_RESOURCE_RECALC_DUEDATE_URL);

		verify(mockManagementService).recalculateJobDuedate(MockProvider.EXAMPLE_JOB_ID, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRecalculateDuedateWithUnknownJobId()
	  public virtual void testRecalculateDuedateWithUnknownJobId()
	  {
		string jobId = MockProvider.NON_EXISTING_JOB_ID;

		string expectedMessage = "No job found with id '" + jobId + "'.";

		doThrow(new NotFoundException(expectedMessage)).when(mockManagementService).recalculateJobDuedate(jobId, true);

		given().pathParam("id", jobId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(expectedMessage)).when().post(JOB_RESOURCE_RECALC_DUEDATE_URL);

		verify(mockManagementService).recalculateJobDuedate(jobId, true);
		verifyNoMoreInteractions(mockManagementService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRecalculateDuedateUnauthorized()
	  public virtual void testRecalculateDuedateUnauthorized()
	  {
		string jobId = MockProvider.EXAMPLE_JOB_ID;

		string expectedMessage = "Missing permissions";

		doThrow(new AuthorizationException(expectedMessage)).when(mockManagementService).recalculateJobDuedate(jobId, true);

		given().pathParam("id", jobId).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().post(JOB_RESOURCE_RECALC_DUEDATE_URL);

		verify(mockManagementService).recalculateJobDuedate(jobId, true);
		verifyNoMoreInteractions(mockManagementService);
	  }

	  protected internal virtual void verifyBatchJson(string batchJson)
	  {
		BatchDto batch = JsonPathUtil.from(batchJson).getObject("", typeof(BatchDto));
		assertNotNull("The returned batch should not be null.", batch);
		assertEquals(MockProvider.EXAMPLE_BATCH_ID, batch.Id);
		assertEquals(MockProvider.EXAMPLE_BATCH_TYPE, batch.Type);
		assertEquals(MockProvider.EXAMPLE_BATCH_TOTAL_JOBS, batch.TotalJobs);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOBS_PER_SEED, batch.BatchJobsPerSeed);
		assertEquals(MockProvider.EXAMPLE_INVOCATIONS_PER_BATCH_JOB, batch.InvocationsPerBatchJob);
		assertEquals(MockProvider.EXAMPLE_SEED_JOB_DEFINITION_ID, batch.SeedJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_MONITOR_JOB_DEFINITION_ID, batch.MonitorJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_BATCH_JOB_DEFINITION_ID, batch.BatchJobDefinitionId);
		assertEquals(MockProvider.EXAMPLE_TENANT_ID, batch.TenantId);
	  }

	}

}