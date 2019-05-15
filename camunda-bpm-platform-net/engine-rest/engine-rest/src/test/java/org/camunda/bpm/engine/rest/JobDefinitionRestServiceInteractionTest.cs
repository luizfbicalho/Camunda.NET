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
//	import static io.restassured.path.json.JsonPath.from;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
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
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using JobDefinitionQuery = org.camunda.bpm.engine.management.JobDefinitionQuery;
	using UpdateJobDefinitionSuspensionStateSelectBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateSelectBuilder;
	using UpdateJobDefinitionSuspensionStateTenantBuilder = org.camunda.bpm.engine.management.UpdateJobDefinitionSuspensionStateTenantBuilder;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using MockJobDefinitionBuilder = org.camunda.bpm.engine.rest.helper.MockJobDefinitionBuilder;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class JobDefinitionRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string JOB_DEFINITION_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/job-definition";
	  protected internal static readonly string SINGLE_JOB_DEFINITION_RESOURCE_URL = JOB_DEFINITION_RESOURCE_URL + "/{id}";
	  protected internal static readonly string SINGLE_JOB_DEFINITION_SUSPENDED_URL = SINGLE_JOB_DEFINITION_RESOURCE_URL + "/suspended";
	  protected internal static readonly string JOB_DEFINITION_SUSPENDED_URL = JOB_DEFINITION_RESOURCE_URL + "/suspended";
	  protected internal static readonly string JOB_DEFINITION_RETRIES_URL = SINGLE_JOB_DEFINITION_RESOURCE_URL + "/retries";
	  protected internal static readonly string JOB_DEFINITION_PRIORITY_URL = SINGLE_JOB_DEFINITION_RESOURCE_URL + "/jobPriority";

	  private ProcessEngine namedProcessEngine;
	  private ManagementService mockManagementService;

	  private UpdateJobDefinitionSuspensionStateTenantBuilder mockSuspensionStateBuilder;
	  private UpdateJobDefinitionSuspensionStateSelectBuilder mockSuspensionStateSelectBuilder;

	  private JobDefinitionQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockManagementService = mock(typeof(ManagementService));

		namedProcessEngine = getProcessEngine(MockProvider.EXAMPLE_PROCESS_ENGINE_NAME);
		when(namedProcessEngine.ManagementService).thenReturn(mockManagementService);

		IList<JobDefinition> mockJobDefinitions = Collections.singletonList(MockProvider.createMockJobDefinition());
		mockQuery = setUpMockJobDefinitionQuery(mockJobDefinitions);

		mockSuspensionStateSelectBuilder = mock(typeof(UpdateJobDefinitionSuspensionStateSelectBuilder));
		when(mockManagementService.updateJobDefinitionSuspensionState()).thenReturn(mockSuspensionStateSelectBuilder);

		mockSuspensionStateBuilder = mock(typeof(UpdateJobDefinitionSuspensionStateTenantBuilder));
		when(mockSuspensionStateSelectBuilder.byJobDefinitionId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byProcessDefinitionId(anyString())).thenReturn(mockSuspensionStateBuilder);
		when(mockSuspensionStateSelectBuilder.byProcessDefinitionKey(anyString())).thenReturn(mockSuspensionStateBuilder);
	  }

	  private JobDefinitionQuery setUpMockJobDefinitionQuery(IList<JobDefinition> mockedJobDefinitions)
	  {
		JobDefinitionQuery sampleJobDefinitionQuery = mock(typeof(JobDefinitionQuery));

		when(sampleJobDefinitionQuery.list()).thenReturn(mockedJobDefinitions);
		when(sampleJobDefinitionQuery.count()).thenReturn((long) mockedJobDefinitions.Count);
		if (mockedJobDefinitions.Count == 1)
		{
		  when(sampleJobDefinitionQuery.singleResult()).thenReturn(mockedJobDefinitions[0]);
		}

		when(sampleJobDefinitionQuery.jobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID)).thenReturn(sampleJobDefinitionQuery);

		when(processEngine.ManagementService.createJobDefinitionQuery()).thenReturn(sampleJobDefinitionQuery);
		when(mockManagementService.createJobDefinitionQuery()).thenReturn(sampleJobDefinitionQuery);

		return sampleJobDefinitionQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleJobDefinitionGet()
	  public virtual void testSimpleJobDefinitionGet()
	  {
		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_JOB_DEFINITION_ID)).body("processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("processDefinitionKey", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("jobType", equalTo(MockProvider.EXAMPLE_JOB_TYPE)).body("jobConfiguration", equalTo(MockProvider.EXAMPLE_JOB_CONFIG)).body("activityId", equalTo(MockProvider.EXAMPLE_ACTIVITY_ID)).body("suspended", equalTo(MockProvider.EXAMPLE_JOB_DEFINITION_IS_SUSPENDED)).body("overridingJobPriority", equalTo(MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY)).body("tenantId", equalTo(null)).when().get(SINGLE_JOB_DEFINITION_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).jobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		inOrder.verify(mockQuery).singleResult();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionGetNullJobPriority()
	  public virtual void testJobDefinitionGetNullJobPriority()
	  {
		// given
		JobDefinition mockJobDefinition = (new MockJobDefinitionBuilder()).id(MockProvider.EXAMPLE_JOB_DEFINITION_ID).jobPriority(null).build();

		when(mockQuery.singleResult()).thenReturn(mockJobDefinition);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(MockProvider.EXAMPLE_JOB_DEFINITION_ID)).body("jobPriority", nullValue()).when().get(SINGLE_JOB_DEFINITION_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testJobDefinitionGetIdDoesntExist()
	  public virtual void testJobDefinitionGetIdDoesntExist()
	  {
		JobDefinitionQuery invalidQueryNonExistingJobDefinition;
		invalidQueryNonExistingJobDefinition = mock(typeof(JobDefinitionQuery));

		when(mockManagementService.createJobDefinitionQuery().jobDefinitionId(MockProvider.NON_EXISTING_JOB_DEFINITION_ID)).thenReturn(invalidQueryNonExistingJobDefinition);

		when(invalidQueryNonExistingJobDefinition.singleResult()).thenReturn(null);

		string jobDefinitionId = MockProvider.NON_EXISTING_JOB_DEFINITION_ID;

		given().pathParam("id", jobDefinitionId).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Job Definition with id " + jobDefinitionId + " does not exist")).when().get(SINGLE_JOB_DEFINITION_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionExcludingInstances()
	  public virtual void testActivateJobDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = false;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionExcludingInstances()
	  public virtual void testDelayedActivateJobDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(false);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionIncludingInstances()
	  public virtual void testActivateJobDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionIncludingInstances()
	  public virtual void testDelayedActivateJobDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateThrowProcessEngineException()
	  public virtual void testActivateThrowProcessEngineException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateNonParseableDateFormat()
	  public virtual void testActivateNonParseableDateFormat()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Job Definition with id " + MockProvider.NON_EXISTING_JOB_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByIdThrowsAuthorizationException()
	  public virtual void testActivateJobDefinitionByIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = false;

		string expectedMessage = "expectedMessage";
		doThrow(new AuthorizationException(expectedMessage)).when(mockSuspensionStateBuilder).activate();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionExcludingInstances()
	  public virtual void testSuspendJobDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = false;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).includeJobs(false);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionExcludingInstances()
	  public virtual void testDelayedSuspendJobDefinitionExcludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = false;
		@params["executionDate"] = MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(false);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionIncludingInstances()
	  public virtual void testSuspendJobDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionIncludingInstances()
	  public virtual void testDelayedSuspendJobDefinitionIncludingInstances()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;
		@params["executionDate"] = MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_JOB_DEFINITION_DELAYED_EXECUTION);

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendThrowsProcessEngineException()
	  public virtual void testSuspendThrowsProcessEngineException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = false;

		string expectedMessage = "expectedMessage";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendNonParseableDateFormat()
	  public virtual void testSuspendNonParseableDateFormat()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = false;
		@params["executionDate"] = "a";

		string expectedMessage = "Invalid format: \"a\"";
		string exceptionMessage = "The suspension state of Job Definition with id " + MockProvider.NON_EXISTING_JOB_DEFINITION_ID + " could not be updated due to: " + expectedMessage;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(exceptionMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendWithMultipleByParameters()
	  public virtual void testSuspendWithMultipleByParameters()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string message = "Only one of jobDefinitionId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByIdThrowsAuthorizationException()
	  public virtual void testSuspendJobDefinitionByIdThrowsAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = false;

		string expectedMessage = "expectedMessage";
		doThrow(new AuthorizationException(expectedMessage)).when(mockSuspensionStateBuilder).suspend();

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedMessage)).when().put(SINGLE_JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKey()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionByProcessDefinitionKey()
	  public virtual void testDelayedActivateJobDefinitionByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  public virtual void testDelayedActivateJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyWithUnparseableDate()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Job Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyWithException()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyWithAuthorizationException()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyWithAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyAndTenantId()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testActivateJobDefinitionByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKey()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionByProcessDefinitionKey()
	  public virtual void testDelayedSuspendJobDefinitionByProcessDefinitionKey()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  public virtual void testDelayedSuspendJobDefinitionByProcessDefinitionKeyIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyWithUnparseableDate()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Job Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyWithException()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyWithAuthorizationException()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyWithAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyAndTenantId()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyAndTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionTenantId"] = MockProvider.EXAMPLE_TENANT_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionTenantId(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionKeyWithoutTenantId()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionKeyWithoutTenantId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
		@params["processDefinitionWithoutTenantId"] = true;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		verify(mockSuspensionStateBuilder).processDefinitionWithoutTenantId();
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionId()
	  public virtual void testActivateJobDefinitionByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  public virtual void testActivateJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionByProcessDefinitionId()
	  public virtual void testDelayedActivateJobDefinitionByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedActivateJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  public virtual void testDelayedActivateJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["includeJobs"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).activate();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionIdWithUnparseableDate()
	  public virtual void testActivateJobDefinitionByProcessDefinitionIdWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Job Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionIdWithException()
	  public virtual void testActivateJobDefinitionByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByProcessDefinitionIdWithAuthorizationException()
	  public virtual void testActivateJobDefinitionByProcessDefinitionIdWithAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).activate();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionId()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionByProcessDefinitionId()
	  public virtual void testDelayedSuspendJobDefinitionByProcessDefinitionId()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelayedSuspendJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  public virtual void testDelayedSuspendJobDefinitionByProcessDefinitionIdIncludingInstaces()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["includeJobs"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION;

		DateTime executionDate = DateTimeUtil.parseDate(MockProvider.EXAMPLE_PROCESS_DEFINITION_DELAYED_EXECUTION);

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_SUSPENDED_URL);

		verify(mockSuspensionStateSelectBuilder).byProcessDefinitionId(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		verify(mockSuspensionStateBuilder).executionDate(executionDate);
		verify(mockSuspensionStateBuilder).includeJobs(true);
		verify(mockSuspensionStateBuilder).suspend();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionIdWithUnparseableDate()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionIdWithUnparseableDate()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
		@params["executionDate"] = "a";

		string message = "Could not update the suspension state of Job Definitions due to: Invalid format: \"a\"";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionIdWithException()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionIdWithException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new ProcessEngineException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", @is(typeof(ProcessEngineException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByProcessDefinitionIdWithAuthorizationException()
	  public virtual void testSuspendJobDefinitionByProcessDefinitionIdWithAuthorizationException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;

		string expectedException = "expectedException";
		doThrow(new AuthorizationException(expectedException)).when(mockSuspensionStateBuilder).suspend();

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", @is(typeof(AuthorizationException).Name)).body("message", @is(expectedException)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivateJobDefinitionByIdShouldThrowException()
	  public virtual void testActivateJobDefinitionByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = false;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByIdShouldThrowException()
	  public virtual void testSuspendJobDefinitionByIdShouldThrowException()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;
		@params["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;

		string message = "Either processDefinitionId or processDefinitionKey can be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuspendJobDefinitionByNothing()
	  public virtual void testSuspendJobDefinitionByNothing()
	  {
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["suspended"] = true;

		string message = "Either jobDefinitionId, processDefinitionId or processDefinitionKey should be set to update the suspension state.";

		given().contentType(ContentType.JSON).body(@params).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", @is(typeof(InvalidRequestException).Name)).body("message", @is(message)).when().put(JOB_DEFINITION_SUSPENDED_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetries()
	  public virtual void testSetJobRetries()
	  {
		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_JOB_RETRIES;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_RETRIES_URL);

		verify(mockManagementService).setJobRetriesByJobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesExceptionExpected()
	  public virtual void testSetJobRetriesExceptionExpected()
	  {
		string expectedMessage = "expected exception message";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setJobRetriesByJobDefinitionId(MockProvider.NON_EXISTING_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_RETRIES);

		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_JOB_RETRIES;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_RETRIES_URL);

		verify(mockManagementService).setJobRetriesByJobDefinitionId(MockProvider.NON_EXISTING_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobRetriesAuthorizationException()
	  public virtual void testSetJobRetriesAuthorizationException()
	  {
		string expectedMessage = "expected exception message";
		doThrow(new AuthorizationException(expectedMessage)).when(mockManagementService).setJobRetriesByJobDefinitionId(MockProvider.NON_EXISTING_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_RETRIES);

		IDictionary<string, object> retriesVariableJson = new Dictionary<string, object>();
		retriesVariableJson["retries"] = MockProvider.EXAMPLE_JOB_RETRIES;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(retriesVariableJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_RETRIES_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriority()
	  public virtual void testSetJobPriority()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_PRIORITY_URL);

		verify(mockManagementService).setOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityToExtremeValue()
	  public virtual void testSetJobPriorityToExtremeValue()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = long.MaxValue;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_PRIORITY_URL);

		verify(mockManagementService).setOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID, long.MaxValue, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityIncludeExistingJobs()
	  public virtual void testSetJobPriorityIncludeExistingJobs()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY;
		priorityJson["includeJobs"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_PRIORITY_URL);

		verify(mockManagementService).setOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY, true);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityExceptionExpected()
	  public virtual void testSetJobPriorityExceptionExpected()
	  {
		string expectedMessage = "expected exception message";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).setOverridingJobPriorityForJobDefinition(eq(MockProvider.EXAMPLE_JOB_DEFINITION_ID), eq(MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY), anyBoolean());

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);

		verify(mockManagementService).setOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID, MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY, false);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetNonExistingJobDefinitionPriority()
	  public virtual void testSetNonExistingJobDefinitionPriority()
	  {
		string expectedMessage = "expected exception message";

		doThrow(new NotFoundException(expectedMessage)).when(mockManagementService).setOverridingJobPriorityForJobDefinition(eq(MockProvider.NON_EXISTING_JOB_DEFINITION_ID), eq(MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY), anyBoolean());

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobPriorityAuthorizationException()
	  public virtual void testSetJobPriorityAuthorizationException()
	  {
		string expectedMessage = "expected exception message";
		doThrow(new AuthorizationException(expectedMessage)).when(mockManagementService).setOverridingJobPriorityForJobDefinition(eq(MockProvider.EXAMPLE_JOB_DEFINITION_ID), eq(MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY), anyBoolean());

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = MockProvider.EXAMPLE_JOB_DEFINITION_PRIORITY;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResetJobPriority()
	  public virtual void testResetJobPriority()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(JOB_DEFINITION_PRIORITY_URL);

		verify(mockManagementService).clearOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResetJobPriorityIncludeJobsNotAllowed()
	  public virtual void testResetJobPriorityIncludeJobsNotAllowed()
	  {
		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;
		priorityJson["includeJobs"] = true;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot reset priority for job definition " + MockProvider.EXAMPLE_JOB_DEFINITION_ID + " with includeJobs=true")).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResetJobPriorityExceptionExpected()
	  public virtual void testResetJobPriorityExceptionExpected()
	  {
		string expectedMessage = "expected exception message";

		doThrow(new ProcessEngineException(expectedMessage)).when(mockManagementService).clearOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID);

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).body("type", equalTo(typeof(RestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResetNonExistingJobDefinitionPriority()
	  public virtual void testResetNonExistingJobDefinitionPriority()
	  {
		string expectedMessage = "expected exception message";

		doThrow(new NotFoundException(expectedMessage)).when(mockManagementService).clearOverridingJobPriorityForJobDefinition(MockProvider.NON_EXISTING_JOB_DEFINITION_ID);

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;

		given().pathParam("id", MockProvider.NON_EXISTING_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.NOT_FOUND.StatusCode).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testResetJobPriorityAuthorizationException()
	  public virtual void testResetJobPriorityAuthorizationException()
	  {
		string expectedMessage = "expected exception message";
		doThrow(new AuthorizationException(expectedMessage)).when(mockManagementService).clearOverridingJobPriorityForJobDefinition(MockProvider.EXAMPLE_JOB_DEFINITION_ID);

		IDictionary<string, object> priorityJson = new Dictionary<string, object>();
		priorityJson["priority"] = null;

		given().pathParam("id", MockProvider.EXAMPLE_JOB_DEFINITION_ID).contentType(ContentType.JSON).body(priorityJson).then().expect().statusCode(Status.FORBIDDEN.StatusCode).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(expectedMessage)).when().put(JOB_DEFINITION_PRIORITY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockQuery = setUpMockJobDefinitionQuery(createMockJobDefinitionsTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(JOB_DEFINITION_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> jobDefinitions = from(content).getList("");
		assertThat(jobDefinitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockQuery = setUpMockJobDefinitionQuery(createMockJobDefinitionsTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOB_DEFINITION_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> jobDefinitions = from(content).getList("");
		assertThat(jobDefinitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

	  private IList<JobDefinition> createMockJobDefinitionsTwoTenants()
	  {
		return Arrays.asList(MockProvider.mockJobDefinition().tenantId(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockJobDefinition().tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
	  }

	}

}