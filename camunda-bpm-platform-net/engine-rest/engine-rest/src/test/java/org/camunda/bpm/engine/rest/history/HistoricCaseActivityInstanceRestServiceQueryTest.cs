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


	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricCaseActivityInstanceRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/case-activity-instance";

	  protected internal static readonly string HISTORIC_CASE_ACTIVITY_INSTANCE_COUNT_RESOURCE_URL = HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL + "/count";

	  protected internal HistoricCaseActivityInstanceQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricCaseActivityInstanceQuery(MockProvider.createMockHistoricCaseActivityInstances());
	  }

	  protected internal virtual HistoricCaseActivityInstanceQuery setUpMockHistoricCaseActivityInstanceQuery(IList<HistoricCaseActivityInstance> mockedHistoricCaseActivityInstances)
	  {
		HistoricCaseActivityInstanceQuery mockedHistoricCaseActivityInstanceQuery = mock(typeof(HistoricCaseActivityInstanceQuery));
		when(mockedHistoricCaseActivityInstanceQuery.list()).thenReturn(mockedHistoricCaseActivityInstances);
		when(mockedHistoricCaseActivityInstanceQuery.count()).thenReturn((long) mockedHistoricCaseActivityInstances.Count);

		when(processEngine.HistoryService.createHistoricCaseActivityInstanceQuery()).thenReturn(mockedHistoricCaseActivityInstanceQuery);

		return mockedHistoricCaseActivityInstanceQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("caseInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "caseInstanceId").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityInstanceId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityInstanceId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceId();
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
		executeAndVerifySorting("caseActivityId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityName", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityName();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityName", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityName();
		inOrder.verify(mockedQuery).desc();

		executeAndVerifySorting("caseActivityType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("caseActivityType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCaseActivityType();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("createTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceCreateTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("createTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceCreateTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceEndTime();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("endTime", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceEndTime();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceDuration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("duration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByHistoricCaseActivityInstanceDuration();
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
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTenantId();
		inOrder.verify(mockedQuery).desc();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_COUNT_RESOURCE_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricActivityQuery()
	  public virtual void testSimpleHistoricActivityQuery()
	  {
		string caseInstanceId = MockProvider.EXAMPLE_CASE_INSTANCE_ID;

		Response response = given().queryParam("caseInstanceId", caseInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).caseInstanceId(caseInstanceId);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedParentCaseActivityInstanceId = from(content).getString("[0].parentCaseActivityInstanceId");
		string returnedCaseActivityId = from(content).getString("[0].caseActivityId");
		string returnedCaseActivityName = from(content).getString("[0].caseActivityName");
		string returnedCaseActivityType = from(content).getString("[0].caseActivityType");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedCaseExecutionId = from(content).getString("[0].caseExecutionId");
		string returnedTaskId = from(content).getString("[0].taskId");
		string returnedCalledProcessInstanceId = from(content).getString("[0].calledProcessInstanceId");
		string returnedCalledCaseInstanceId = from(content).getString("[0].calledCaseInstanceId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		DateTime returnedCreateTime = DateTimeUtil.parseDate(from(content).getString("[0].createTime"));
		DateTime returnedEndTime = DateTimeUtil.parseDate(from(content).getString("[0].endTime"));
		long returnedDurationInMillis = from(content).getLong("[0].durationInMillis");
		bool required = from(content).getBoolean("[0].required");
		bool available = from(content).getBoolean("[0].available");
		bool enabled = from(content).getBoolean("[0].enabled");
		bool disabled = from(content).getBoolean("[0].disabled");
		bool active = from(content).getBoolean("[0].active");
		bool completed = from(content).getBoolean("[0].completed");
		bool terminated = from(content).getBoolean("[0].terminated");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_PARENT_CASE_ACTIVITY_INSTANCE_ID, returnedParentCaseActivityInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_ID, returnedCaseActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_NAME, returnedCaseActivityName);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_TYPE, returnedCaseActivityType);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_EXECUTION_ID, returnedCaseExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ID, returnedTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID, returnedCalledProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID, returnedCalledCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME), returnedCreateTime);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME), returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_DURATION, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_REQUIRED, required);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_AVAILABLE, available);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ENABLED, enabled);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_DISABLED, disabled);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ACTIVE, active);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_COMPLETED, completed);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_TERMINATED, terminated);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyStringParameterQueryInvocations();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testBooleanParameters()
	  public virtual void testBooleanParameters()
	  {
		IDictionary<string, bool> @params = CompleteBooleanQueryParameters;

		given().queryParams(@params).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyBooleanParameterQueryInvocations();
	  }

	  protected internal virtual IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["caseActivityInstanceId"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID;
			parameters["caseInstanceId"] = MockProvider.EXAMPLE_CASE_INSTANCE_ID;
			parameters["caseDefinitionId"] = MockProvider.EXAMPLE_CASE_DEFINITION_ID;
			parameters["caseExecutionId"] = MockProvider.EXAMPLE_CASE_EXECUTION_ID;
			parameters["caseActivityId"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_ID;
			parameters["caseActivityName"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_NAME;
			parameters["caseActivityType"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_TYPE;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockedQuery).caseActivityInstanceId(stringQueryParameters["caseActivityInstanceId"]);
		verify(mockedQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);
		verify(mockedQuery).caseDefinitionId(stringQueryParameters["caseDefinitionId"]);
		verify(mockedQuery).caseExecutionId(stringQueryParameters["caseExecutionId"]);
		verify(mockedQuery).caseActivityId(stringQueryParameters["caseActivityId"]);
		verify(mockedQuery).caseActivityName(stringQueryParameters["caseActivityName"]);
		verify(mockedQuery).caseActivityType(stringQueryParameters["caseActivityType"]);

		verify(mockedQuery).list();
	  }

	  protected internal virtual IDictionary<string, bool> CompleteBooleanQueryParameters
	  {
		  get
		  {
			IDictionary<string, bool> parameters = new Dictionary<string, bool>();
    
			parameters["required"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_REQUIRED;
			parameters["finished"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_FINISHED;
			parameters["unfinished"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_UNFINISHED;
			parameters["available"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_AVAILABLE;
			parameters["enabled"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ENABLED;
			parameters["disabled"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_DISABLED;
			parameters["active"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_ACTIVE;
			parameters["completed"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_COMPLETED;
			parameters["terminated"] = MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_IS_TERMINATED;
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyBooleanParameterQueryInvocations()
	  {
		IDictionary<string, bool> booleanParams = CompleteBooleanQueryParameters;
		bool? required = booleanParams["required"];
		bool? finished = booleanParams["finished"];
		bool? unfinished = booleanParams["unfinished"];
		bool? available = booleanParams["available"];
		bool? enabled = booleanParams["enabled"];
		bool? disabled = booleanParams["disabled"];
		bool? active = booleanParams["active"];
		bool? completed = booleanParams["completed"];
		bool? terminated = booleanParams["terminated"];

		if (required != null && required)
		{
		  verify(mockedQuery).required();
		}
		if (finished != null && finished)
		{
		  verify(mockedQuery).ended();
		}
		if (unfinished != null && unfinished)
		{
		  verify(mockedQuery).notEnded();
		}
		if (available != null && available)
		{
		  verify(mockedQuery).available();
		}
		if (enabled != null && enabled)
		{
		  verify(mockedQuery).enabled();
		}
		if (disabled != null && disabled)
		{
		  verify(mockedQuery).disabled();
		}
		if (active != null && active)
		{
		  verify(mockedQuery).active();
		}
		if (completed != null && completed)
		{
		  verify(mockedQuery).completed();
		}
		if (terminated != null && terminated)
		{
		  verify(mockedQuery).terminated();
		}

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFinishedHistoricCaseActivityQuery()
	  public virtual void testFinishedHistoricCaseActivityQuery()
	  {
		Response response = given().queryParam("finished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).ended();
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME, returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUnfinishedHistoricCaseActivityQuery()
	  public virtual void testUnfinishedHistoricCaseActivityQuery()
	  {
		IList<HistoricCaseActivityInstance> mockedHistoricCaseActivityInstances = MockProvider.createMockRunningHistoricCaseActivityInstances();
		HistoricCaseActivityInstanceQuery mockedHistoricCaseActivityInstanceQuery = mock(typeof(HistoricCaseActivityInstanceQuery));
		when(mockedHistoricCaseActivityInstanceQuery.list()).thenReturn(mockedHistoricCaseActivityInstances);
		when(processEngine.HistoryService.createHistoricCaseActivityInstanceQuery()).thenReturn(mockedHistoricCaseActivityInstanceQuery);

		Response response = given().queryParam("unfinished", true).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		InOrder inOrder = inOrder(mockedHistoricCaseActivityInstanceQuery);
		inOrder.verify(mockedHistoricCaseActivityInstanceQuery).notEnded();
		inOrder.verify(mockedHistoricCaseActivityInstanceQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals(1, instances.Count);
		Assert.assertNotNull(instances[0]);

		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedActivityEndTime = from(content).getString("[0].endTime");

		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertNull(returnedActivityEndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeCreateTimeQuery()
	  public virtual void testHistoricAfterAndBeforeCreateTimeQuery()
	  {
		given().queryParam("createdAfter", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_AFTER).queryParam("createdBefore", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyCreateParameterQueryInvocations();
	  }

	  protected internal virtual IDictionary<string, DateTime> CompleteCreateDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["createdAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_AFTER);
			parameters["createdBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATED_BEFORE);
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyCreateParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> startDateParameters = CompleteCreateDateQueryParameters;

		verify(mockedQuery).createdAfter(startDateParameters["createdAfter"]);
		verify(mockedQuery).createdBefore(startDateParameters["createdBefore"]);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHistoricAfterAndBeforeEndTimeQuery()
	  public virtual void testHistoricAfterAndBeforeEndTimeQuery()
	  {
		given().queryParam("endedAfter", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_AFTER).queryParam("endedBefore", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_BEFORE).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verifyEndedParameterQueryInvocations();
	  }

	  protected internal virtual IDictionary<string, DateTime> CompleteEndedDateQueryParameters
	  {
		  get
		  {
			IDictionary<string, DateTime> parameters = new Dictionary<string, DateTime>();
    
			parameters["endedAfter"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_AFTER);
			parameters["endedBefore"] = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ENDED_BEFORE);
    
			return parameters;
		  }
	  }

	  protected internal virtual void verifyEndedParameterQueryInvocations()
	  {
		IDictionary<string, DateTime> finishedDateParameters = CompleteEndedDateQueryParameters;

		verify(mockedQuery).endedAfter(finishedDateParameters["endedAfter"]);
		verify(mockedQuery).endedBefore(finishedDateParameters["endedBefore"]);

		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTenantIdListParameter()
	  public virtual void testTenantIdListParameter()
	  {
		mockedQuery = setUpMockHistoricCaseActivityInstanceQuery(createMockHistoricCaseActivityInstancesTwoTenants());

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).tenantIdIn(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
		verify(mockedQuery).list();

		string content = response.asString();
		IList<string> historicCaseActivityInstances = from(content).getList("");
		assertThat(historicCaseActivityInstances).hasSize(2);

		string returnedTenantId1 = from(content).getString("[0].tenantId");
		string returnedTenantId2 = from(content).getString("[1].tenantId");

		assertThat(returnedTenantId1).isEqualTo(MockProvider.EXAMPLE_TENANT_ID);
		assertThat(returnedTenantId2).isEqualTo(MockProvider.ANOTHER_EXAMPLE_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActivityInstanceIdListParameter()
	  public virtual void testCaseActivityInstanceIdListParameter()
	  {

		given().queryParam("caseActivityInstanceIdIn", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID + "," + MockProvider.EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityInstanceIdIn(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID, MockProvider.EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_INSTANCE_ID);
		verify(mockedQuery).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseActivityIdListParameter()
	  public virtual void testCaseActivityIdListParameter()
	  {

		given().queryParam("caseActivityIdIn", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_ID + "," + MockProvider.EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_CASE_ACTIVITY_INSTANCE_RESOURCE_URL);

		verify(mockedQuery).caseActivityIdIn(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_ID, MockProvider.EXAMPLE_HISTORIC_ANOTHER_CASE_ACTIVITY_ID);
		verify(mockedQuery).list();
	  }

	  private IList<HistoricCaseActivityInstance> createMockHistoricCaseActivityInstancesTwoTenants()
	  {
		return Arrays.asList(MockProvider.createMockHistoricCaseActivityInstance(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricCaseActivityInstance(MockProvider.ANOTHER_EXAMPLE_TENANT_ID));
	  }

	}

}