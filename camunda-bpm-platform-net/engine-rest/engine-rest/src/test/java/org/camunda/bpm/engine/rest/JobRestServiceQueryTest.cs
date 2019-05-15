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
//	import static io.restassured.RestAssured.expect;
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
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
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

	public class JobRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string JOBS_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/job";
	  protected internal static readonly string JOBS_QUERY_COUNT_URL = JOBS_RESOURCE_URL + "/count";

	  private JobQuery mockQuery;
	  private const int MAX_RESULTS_TEN = 10;
	  private const int FIRST_RESULTS_ZERO = 0;
	  protected internal static readonly long JOB_QUERY_MAX_PRIORITY = long.MaxValue;
	  protected internal static readonly long JOB_QUERY_MIN_PRIORITY = long.MinValue;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = setUpMockJobQuery(MockProvider.createMockJobs());
	  }

	  private JobQuery setUpMockJobQuery(IList<Job> mockedJobs)
	  {
		JobQuery sampleJobQuery = mock(typeof(JobQuery));

		when(sampleJobQuery.list()).thenReturn(mockedJobs);
		when(sampleJobQuery.count()).thenReturn((long) mockedJobs.Count);

		when(processEngine.ManagementService.createJobQuery()).thenReturn(sampleJobQuery);

		return sampleJobQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryJobId = "";
		given().queryParam("id", queryJobId).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).list();
		verifyNoMoreInteractions(mockQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "jobDueDate").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleJobQuery()
	  public virtual void testSimpleJobQuery()
	  {
		string jobId = MockProvider.EXAMPLE_JOB_ID;

		Response response = given().queryParam("jobId", jobId).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).jobId(jobId);
		inOrder.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one job returned.", 1, instances.Count);
		Assert.assertNotNull("The returned job should not be null.", instances[0]);

		string returnedJobId = from(content).getString("[0].id");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedExceptionMessage = from(content).getString("[0].exceptionMessage");
		int returnedRetries = from(content).getInt("[0].retries");
		DateTime returnedDueDate = DateTimeUtil.parseDate(from(content).getString("[0].dueDate"));
		bool returnedSuspended = from(content).getBoolean("[0].suspended");
		long returnedPriority = from(content).getLong("[0].priority");
		string returnedJobDefinitionId = from(content).getString("[0].jobDefinitionId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		string returnedCreateTime = from(content).getString("[0].createTime");

		Assert.assertEquals(MockProvider.EXAMPLE_JOB_ID, returnedJobId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_NO_EXCEPTION_MESSAGE, returnedExceptionMessage);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_RETRIES, returnedRetries);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_DUE_DATE), returnedDueDate);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_IS_SUSPENDED, returnedSuspended);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_PRIORITY, returnedPriority);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_DEFINITION_ID, returnedJobDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_CREATE_TIME, returnedCreateTime);
	  }

	  private interface DateParameters
	  {
		string name();

		string description();

		void expectLowerThan(JobQuery query, DateTime date);

		void expectHigherThan(JobQuery query, DateTime date);
	  }

	  private static readonly DateParameters DUE_DATES = new DateParametersAnonymousInnerClass();

	  private class DateParametersAnonymousInnerClass : DateParameters
	  {
		  public override string name()
		  {
			return "dueDates";
		  }

		  public override string description()
		  {
			return "due date";
		  }

		  public override void expectLowerThan(JobQuery query, DateTime date)
		  {
			query.duedateLowerThan(date);
		  }

		  public override void expectHigherThan(JobQuery query, DateTime date)
		  {
			query.duedateHigherThan(date);
		  }
	  }

	  private static readonly DateParameters CREATE_TIMES = new DateParametersAnonymousInnerClass2();

	  private class DateParametersAnonymousInnerClass2 : DateParameters
	  {
		  public override string name()
		  {
			return "createTimes";
		  }

		  public override string description()
		  {
			return "create time";
		  }

		  public override void expectLowerThan(JobQuery query, DateTime date)
		  {
			query.createdBefore(date);
		  }

		  public override void expectHigherThan(JobQuery query, DateTime date)
		  {
			query.createdAfter(date);
		  }
	  }

	  private void testInvalidDateComparator(DateParameters parameters)
	  {

		string variableValue = withTimezone("2013-05-05T00:00:00");
		string invalidComparator = "bt";

		string queryValue = invalidComparator + "_" + variableValue;
		given().queryParam(parameters.name(), queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Invalid " + parameters.description() + " comparator specified: " + invalidComparator)).when().get(JOBS_RESOURCE_URL);
	  }

	  private void testInvalidDateComparatorAsPost(DateParameters parameters)
	  {
		string invalidComparator = "bt";

		IDictionary<string, object> conditionJson = new Dictionary<string, object>();
		conditionJson["operator"] = invalidComparator;
		conditionJson["value"] = withTimezone("2013-05-05T00:00:00");

		IList<IDictionary<string, object>> conditions = new List<IDictionary<string, object>>();
		conditions.Add(conditionJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json[parameters.name()] = conditions;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Invalid " + parameters.description() + " comparator specified: " + invalidComparator)).when().post(JOBS_RESOURCE_URL);
	  }

	  private void testInvalidDate(DateParameters parameters)
	  {
		string variableValue = "invalidValue";
		string invalidComparator = "lt";

		string queryValue = invalidComparator + "_" + variableValue;
		given().queryParam(parameters.name(), queryValue).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Invalid " + parameters.description() + " format: Cannot convert value \"invalidValue\" to java type java.util.Date")).when().get(JOBS_RESOURCE_URL);
	  }

	  private void testInvalidDateAsPost(DateParameters parameters)
	  {
		IDictionary<string, object> conditionJson = new Dictionary<string, object>();
		conditionJson["operator"] = "lt";
		conditionJson["value"] = "invalidValue";

		IList<IDictionary<string, object>> conditions = new List<IDictionary<string, object>>();
		conditions.Add(conditionJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json[parameters.name()] = conditions;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Invalid " + parameters.description() + " format: Cannot convert value \"invalidValue\" to java type java.util.Date")).when().post(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDueDateComparator()
	  public virtual void testInvalidDueDateComparator()
	  {
		testInvalidDateComparator(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCreateTimeComparator()
	  public virtual void testInvalidCreateTimeComparator()
	  {
		testInvalidDateComparator(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDueDateComperatorAsPost()
	  public virtual void testInvalidDueDateComperatorAsPost()
	  {
		testInvalidDateComparatorAsPost(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCreateTimeComparatorAsPost()
	  public virtual void testInvalidCreateTimeComparatorAsPost()
	  {
		testInvalidDateComparatorAsPost(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDueDate()
	  public virtual void testInvalidDueDate()
	  {
		testInvalidDate(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCreateTime()
	  public virtual void testInvalidCreateTime()
	  {
		testInvalidDate(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDueDateAsPost()
	  public virtual void testInvalidDueDateAsPost()
	  {
		testInvalidDateAsPost(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidCreateTimeAsPost()
	  public virtual void testInvalidCreateTimeAsPost()
	  {
		testInvalidDateAsPost(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingDueDates()
	  public virtual void testAdditionalParametersExcludingDueDates()
	  {
		IDictionary<string, object> parameters = CompleteParameters;

		given().queryParams(parameters).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verifyParameterQueryInvocations();
		verify(mockQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessagesParameter()
	  public virtual void testMessagesParameter()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["messages"] = MockProvider.EXAMPLE_MESSAGES;

		given().queryParams(parameters).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).messages();
		verify(mockQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessagesTimersParameter()
	  public virtual void testMessagesTimersParameter()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["messages"] = MockProvider.EXAMPLE_MESSAGES;
		parameters["timers"] = MockProvider.EXAMPLE_TIMERS;

		given().queryParams(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type",equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Parameter timers cannot be used together with parameter messages.")).when().get(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessagesTimersParameterAsPost()
	  public virtual void testMessagesTimersParameterAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["messages"] = MockProvider.EXAMPLE_MESSAGES;
		parameters["timers"] = MockProvider.EXAMPLE_TIMERS;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type",equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Parameter timers cannot be used together with parameter messages.")).when().post(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMessagesParameterAsPost()
	  public virtual void testMessagesParameterAsPost()
	  {
		IDictionary<string, object> parameters = new Dictionary<string, object>();
		parameters["messages"] = MockProvider.EXAMPLE_MESSAGES;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		verify(mockQuery).messages();
		verify(mockQuery).list();
	  }

	  private IDictionary<string, object> CompleteParameters
	  {
		  get
		  {
			IDictionary<string, object> parameters = new Dictionary<string, object>();
    
			parameters["activityId"] = MockProvider.EXAMPLE_ACTIVITY_ID;
			parameters["jobId"] = MockProvider.EXAMPLE_JOB_ID;
			parameters["processInstanceId"] = MockProvider.EXAMPLE_PROCESS_INSTANCE_ID;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_ID;
			parameters["processDefinitionKey"] = MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY;
			parameters["executionId"] = MockProvider.EXAMPLE_EXECUTION_ID;
			parameters["withRetriesLeft"] = MockProvider.EXAMPLE_WITH_RETRIES_LEFT;
			parameters["executable"] = MockProvider.EXAMPLE_EXECUTABLE;
			parameters["timers"] = MockProvider.EXAMPLE_TIMERS;
			parameters["withException"] = MockProvider.EXAMPLE_WITH_EXCEPTION;
			parameters["exceptionMessage"] = MockProvider.EXAMPLE_EXCEPTION_MESSAGE;
			parameters["noRetriesLeft"] = MockProvider.EXAMPLE_NO_RETRIES_LEFT;
			parameters["active"] = true;
			parameters["suspended"] = true;
			parameters["priorityLowerThanOrEquals"] = JOB_QUERY_MAX_PRIORITY;
			parameters["priorityHigherThanOrEquals"] = JOB_QUERY_MIN_PRIORITY;
			parameters["jobDefinitionId"] = MockProvider.EXAMPLE_JOB_DEFINITION_ID;
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingDueDatesAsPost()
	  public virtual void testAdditionalParametersExcludingDueDatesAsPost()
	  {
		IDictionary<string, object> parameters = CompleteParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(parameters).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		verifyParameterQueryInvocations();
		verify(mockQuery).list();
	  }


	  private void verifyParameterQueryInvocations()
	  {
		IDictionary<string, object> parameters = CompleteParameters;

		verify(mockQuery).jobId((string) parameters["jobId"]);
		verify(mockQuery).processInstanceId((string) parameters["processInstanceId"]);
		verify(mockQuery).processDefinitionId((string) parameters["processDefinitionId"]);
		verify(mockQuery).processDefinitionKey((string) parameters["processDefinitionKey"]);
		verify(mockQuery).executionId((string) parameters["executionId"]);
		verify(mockQuery).activityId((string) parameters["activityId"]);
		verify(mockQuery).withRetriesLeft();
		verify(mockQuery).executable();
		verify(mockQuery).timers();
		verify(mockQuery).withException();
		verify(mockQuery).exceptionMessage((string) parameters["exceptionMessage"]);
		verify(mockQuery).noRetriesLeft();
		verify(mockQuery).active();
		verify(mockQuery).suspended();
		verify(mockQuery).priorityLowerThanOrEquals(JOB_QUERY_MAX_PRIORITY);
		verify(mockQuery).priorityHigherThanOrEquals(JOB_QUERY_MIN_PRIORITY);
		verify(mockQuery).jobDefinitionId(MockProvider.EXAMPLE_JOB_DEFINITION_ID);
	  }

	  private void testDateParameters(DateParameters parameters)
	  {
		string variableValue = withTimezone("2013-05-05T00:00:00");
		DateTime date = DateTimeUtil.parseDate(variableValue);

		string queryValue = "lt_" + variableValue;
		given().queryParam(parameters.name(), queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		InOrder inOrder = inOrder(mockQuery);
		parameters.expectLowerThan(inOrder.verify(mockQuery), date);
		inOrder.verify(mockQuery).list();

		queryValue = "gt_" + variableValue;
		given().queryParam(parameters.name(), queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		inOrder = inOrder(mockQuery);
		parameters.expectHigherThan(inOrder.verify(mockQuery), date);
		inOrder.verify(mockQuery).list();
	  }

	  private void testDateParametersAsPost(DateParameters parameters)
	  {
		string value = withTimezone("2013-05-18T00:00:00");
		string anotherValue = withTimezone("2013-05-05T00:00:00");

		DateTime date = DateTimeUtil.parseDate(value);
		DateTime anotherDate = DateTimeUtil.parseDate(anotherValue);

		IDictionary<string, object> conditionJson = new Dictionary<string, object>();
		conditionJson["operator"] = "lt";
		conditionJson["value"] = value;

		IDictionary<string, object> anotherConditionJson = new Dictionary<string, object>();
		anotherConditionJson["operator"] = "gt";
		anotherConditionJson["value"] = anotherValue;

		IList<IDictionary<string, object>> conditions = new List<IDictionary<string, object>>();
		conditions.Add(conditionJson);
		conditions.Add(anotherConditionJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json[parameters.name()] = conditions;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		parameters.expectHigherThan(verify(mockQuery), anotherDate);
		parameters.expectLowerThan(verify(mockQuery), date);
	  }

	  private void testMultipleDateParameters(DateParameters parameters)
	  {
		string variableValue1 = withTimezone("2012-05-05T00:00:00");
		string variableParameter1 = "gt_" + variableValue1;

		string variableValue2 = withTimezone("2013-02-02T00:00:00");
		string variableParameter2 = "lt_" + variableValue2;

		DateTime date = DateTimeUtil.parseDate(variableValue1);
		DateTime anotherDate = DateTimeUtil.parseDate(variableValue2);

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam(parameters.name(), queryValue).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		parameters.expectHigherThan(verify(mockQuery), date);
		parameters.expectLowerThan(verify(mockQuery), anotherDate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDueDateParameters()
	  public virtual void testDueDateParameters()
	  {
		testDateParameters(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTimeParameters()
	  public virtual void testCreateTimeParameters()
	  {
		testDateParameters(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDueDateParametersAsPost()
	  public virtual void testDueDateParametersAsPost()
	  {
		testDateParametersAsPost(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateTimeParametersAsPost()
	  public virtual void testCreateTimeParametersAsPost()
	  {
		testDateParametersAsPost(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleDueDateParameters()
	  public virtual void testMultipleDueDateParameters()
	  {
		testMultipleDateParameters(DUE_DATES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleCreateTimeParameters()
	  public virtual void testMultipleCreateTimeParameters()
	  {
		testMultipleDateParameters(CREATE_TIMES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("jobId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByJobId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("processInstanceId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessInstanceId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("executionId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByExecutionId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("jobRetries", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByJobRetries();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("jobDueDate", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByJobDuedate();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("jobPriority", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByJobPriority();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).desc();
	  }

	  private void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(JOBS_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockQuery);
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = OrderingBuilder.create().orderBy("jobRetries").desc().orderBy("jobDueDate").asc().Json;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		inOrder.verify(mockQuery).orderByJobRetries();
		inOrder.verify(mockQuery).desc();
		inOrder.verify(mockQuery).orderByJobDuedate();
		inOrder.verify(mockQuery).asc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = FIRST_RESULTS_ZERO;
		int maxResults = MAX_RESULTS_TEN;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		given().header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(JOBS_QUERY_COUNT_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(JOBS_QUERY_COUNT_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockQuery = setUpMockJobQuery(createMockJobsTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdParameter()
	  public virtual void testWithoutTenantIdParameter()
	  {
		Job mockJob = MockProvider.mockJob().tenantId(null).build();
		mockQuery = setUpMockJobQuery(Arrays.asList(mockJob));

		Response response = given().queryParam("withoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).withoutTenantId();
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(1);

		string returnedTenantId = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeJobsWithoutTenantIdParameter()
	  public virtual void testIncludeJobsWithoutTenantIdParameter()
	  {
		IList<Job> jobs = Arrays.asList(MockProvider.mockJob().tenantId(null).build(), MockProvider.mockJob().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockQuery = setUpMockJobQuery(jobs);

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID).queryParam("includeJobsWithoutTenantId", true).then().expect().statusCode(Status.OK.StatusCode).when().get(JOBS_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockQuery).includeJobsWithoutTenantId();
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListPostParameter()
	  public virtual void testTenantIdListPostParameter()
	  {
		mockQuery = setUpMockJobQuery(createMockJobsTwoTenants());

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true);

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWithoutTenantIdPostParameter()
	  public virtual void testWithoutTenantIdPostParameter()
	  {
		Job mockJob = MockProvider.mockJob().tenantId(null).build();
		mockQuery = setUpMockJobQuery(Arrays.asList(mockJob));

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["withoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		string content = response.asString();
		IList<string> jobs = from(content).getList("");
		assertThat(jobs).hasSize(1);

		string returnedTenantId = from(content).getString("[0].tenantId");
		assertThat(returnedTenantId).isEqualTo(null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncludeJobsWithoutTenantIdPostParameter()
	  public virtual void testIncludeJobsWithoutTenantIdPostParameter()
	  {
		IList<Job> jobs = Arrays.asList(MockProvider.mockJob().tenantId(null).build(), MockProvider.mockJob().tenantId(MockProvider.EXAMPLE_TENANT_ID).build());
		mockQuery = setUpMockJobQuery(jobs);

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		queryParameters["tenantIdIn"] = new string[] {MockProvider.EXAMPLE_TENANT_ID};
		queryParameters["includeJobsWithoutTenantId"] = true;

		Response response = given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).expect().statusCode(Status.OK.StatusCode).when().post(JOBS_RESOURCE_URL);

		verify(mockQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID);
		verify(mockQuery).includeJobsWithoutTenantId();
		verify(mockQuery).list();

		string content = response.asString();
		IList<string> definitions = from(content).getList("");
		assertThat(definitions).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(null);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
	  }

	  private IList<Job> createMockJobsTwoTenants()
	  {
		return Arrays.asList(MockProvider.mockJob().tenantId(MockProvider.EXAMPLE_TENANT_ID).build(), MockProvider.mockJob().tenantId(MockProvider.ANOTHER_EXAMPLE_TENANT_ID).build());
	  }

	}

}