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
//	import static org.mockito.Mockito.when;


	using HistoricIdentityLinkLog = org.camunda.bpm.engine.history.HistoricIdentityLinkLog;
	using HistoricIdentityLinkLogQuery = org.camunda.bpm.engine.history.HistoricIdentityLinkLogQuery;
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

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.expect;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;

	/// 
	/// <summary>
	/// @author Deivarayan Azhagappan
	/// 
	/// </summary>
	public class HistoricIdentityLinkLogRestServiceQueryTest : AbstractRestServiceTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORY_IDENTITY_LINK_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/history/identity-link-log";
	  protected internal static readonly string HISTORY_IDENTITY_LINK_COUNT_QUERY_URL = HISTORY_IDENTITY_LINK_QUERY_URL + "/count";

	  private HistoricIdentityLinkLogQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockedQuery = setUpMockHistoricIdentityLinkQuery(MockProvider.createMockHistoricIdentityLinks());
	  }

	  private HistoricIdentityLinkLogQuery setUpMockHistoricIdentityLinkQuery(IList<HistoricIdentityLinkLog> mockedHistoricIdentityLinks)
	  {

		HistoricIdentityLinkLogQuery mockedHistoricIdentityLinkQuery = mock(typeof(HistoricIdentityLinkLogQuery));
		when(mockedHistoricIdentityLinkQuery.list()).thenReturn(mockedHistoricIdentityLinks);
		when(mockedHistoricIdentityLinkQuery.count()).thenReturn((long) mockedHistoricIdentityLinks.Count);

		when(processEngine.HistoryService.createHistoricIdentityLinkLogQuery()).thenReturn(mockedHistoricIdentityLinkQuery);

		return mockedHistoricIdentityLinkQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("userId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);

		// assignerId
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("assignerId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByAssignerId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("assignerId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByAssignerId();
		inOrder.verify(mockedQuery).desc();

		// userId
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("userId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByUserId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("userId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByUserId();
		inOrder.verify(mockedQuery).desc();

		// groupId
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("groupId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByGroupId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("groupId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByGroupId();
		inOrder.verify(mockedQuery).desc();

		// taskId
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("taskId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByTaskId();
		inOrder.verify(mockedQuery).desc();

		// operationType
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("operationType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByOperationType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("operationType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByOperationType();
		inOrder.verify(mockedQuery).desc();

		// processDefinitionId
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionId();
		inOrder.verify(mockedQuery).desc();

		// processDefinitionKey
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("processDefinitionKey", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByProcessDefinitionKey();
		inOrder.verify(mockedQuery).desc();

		// type
		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("type", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("type", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByType();
		inOrder.verify(mockedQuery).desc();

		// tenantId
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
//ORIGINAL LINE: @Test public void testSimpleHistoricIdentityLinkQuery()
	  public virtual void testSimpleHistoricIdentityLinkQuery()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> identityLinks = from(content).getList("");
		Assert.assertEquals("There should be one incident returned.", 1, identityLinks.Count);
		Assert.assertNotNull("The returned incident should not be null.", identityLinks[0]);

		string returnedAssignerId = from(content).getString("[0].assignerId");
		string returnedTenantId = from(content).getString("[0].tenantId");
		string returnedUserId = from(content).getString("[0].userId");
		string returnedGroupId = from(content).getString("[0].groupId");
		string returnedTaskId = from(content).getString("[0].taskId");
		string returnedType = from(content).getString("[0].type");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessDefinitionKey = from(content).getString("[0].processDefinitionKey");
		string returnedOperationType = from(content).getString("[0].operationType");
		DateTime loggedDate = DateTimeUtil.parseDate(from(content).getString("[0].time"));
		DateTime returnedRemovalTime = DateTimeUtil.parseDate(from(content).getString("[0].removalTime"));
		string returnedRootProcessInstanceId = from(content).getString("[0].rootProcessInstanceId");

		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TIME), loggedDate);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_ASSIGNER_ID, returnedAssignerId);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_USER_ID, returnedUserId);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID, returnedGroupId);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TASK_ID, returnedTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TYPE, returnedType);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_OPERATION_TYPE, returnedOperationType);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_REMOVAL_TIME), returnedRemovalTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_ROOT_PROC_INST_ID, returnedRootProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIds()
	  public virtual void testQueryByTenantIds()
	  {
		mockedQuery = setUpMockHistoricIdentityLinkQuery(Arrays.asList(MockProvider.createMockHistoricIdentityLink(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockHistoricIdentityLink(MockProvider.ANOTHER_EXAMPLE_TENANT_ID)));

		Response response = given().queryParam("tenantIdIn", MockProvider.EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

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
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(HISTORY_IDENTITY_LINK_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByAssignerId()
	  public virtual void testQueryByAssignerId()
	  {
		string assignerId = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_ASSIGNER_ID;

		given().queryParam("assignerId", assignerId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).assignerId(assignerId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByUserId()
	  public virtual void testQueryByUserId()
	  {
		string userId = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_USER_ID;

		given().queryParam("userId", userId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).userId(userId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByGroupId()
	  public virtual void testQueryByGroupId()
	  {
		string groupId = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID;

		given().queryParam("groupId", groupId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).groupId(groupId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTaskId()
	  public virtual void testQueryByTaskId()
	  {
		string taskId = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TASK_ID;

		given().queryParam("taskId", taskId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).taskId(taskId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionId()
	  public virtual void testQueryByProcessDefinitionId()
	  {
		string processDefinitionId = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_ID;

		given().queryParam("processDefinitionId", processDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).processDefinitionId(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionKey()
	  public virtual void testQueryByProcessDefinitionKey()
	  {
		string processDefinitionKey = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_KEY;

		given().queryParam("processDefinitionKey", processDefinitionKey).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).processDefinitionKey(processDefinitionKey);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByType()
	  public virtual void testQueryByType()
	  {
		string type = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TYPE;

		given().queryParam("type", type).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).type(type);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByOperationType()
	  public virtual void testQueryByOperationType()
	  {
		string operationType = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_OPERATION_TYPE;

		given().queryParam("operationType", operationType).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).operationType(operationType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDateBefore()
	  public virtual void testQueryByDateBefore()
	  {
		DateTime dateBefore = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TIME);
		given().queryParam("dateBefore", MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TIME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).dateBefore(dateBefore);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByDateAfter()
	  public virtual void testQueryByDateAfter()
	  {
		DateTime dateAfter = DateTimeUtil.parseDate(MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TIME);

		given().queryParam("dateAfter", MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TIME).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).dateAfter(dateAfter);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantId()
	  public virtual void testQueryByTenantId()
	  {
		string tenantId = MockProvider.EXAMPLE_TENANT_ID;

		given().queryParam("tenantIdIn", tenantId).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verify(mockedQuery).tenantIdIn(tenantId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParameters()
	  public virtual void testAdditionalParameters()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		given().queryParams(stringQueryParameters).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORY_IDENTITY_LINK_QUERY_URL);

		verifyStringParameterQueryInvocations();
	  }
	  protected internal virtual IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["assignerId"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_ASSIGNER_ID;
			parameters["dateBefore"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_DATE_BEFORE;
			parameters["dateAfter"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_DATE_AFTER;
			parameters["userId"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_USER_ID;
			parameters["groupId"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_GROUP_ID;
			parameters["taskId"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TASK_ID;
			parameters["processDefinitionId"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_ID;
			parameters["processDefinitionKey"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_PROC_DEFINITION_KEY;
			parameters["operationType"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_OPERATION_TYPE;
			parameters["tenantIdIn"] = MockProvider.EXAMPLE_TENANT_ID;
			parameters["type"] = MockProvider.EXAMPLE_HIST_IDENTITY_LINK_TYPE;
			return parameters;
		  }
	  }
	  protected internal virtual void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;
		verify(mockedQuery).assignerId(stringQueryParameters["assignerId"]);
		verify(mockedQuery).userId(stringQueryParameters["userId"]);
		verify(mockedQuery).groupId(stringQueryParameters["groupId"]);
		verify(mockedQuery).taskId(stringQueryParameters["taskId"]);
		verify(mockedQuery).dateBefore(DateTimeUtil.parseDate(stringQueryParameters["dateBefore"]));
		verify(mockedQuery).dateAfter(DateTimeUtil.parseDate(stringQueryParameters["dateAfter"]));
		verify(mockedQuery).type(stringQueryParameters["type"]);
		verify(mockedQuery).operationType(stringQueryParameters["operationType"]);
		verify(mockedQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockedQuery).processDefinitionKey(stringQueryParameters["processDefinitionKey"]);
		verify(mockedQuery).tenantIdIn(stringQueryParameters["tenantIdIn"]);
		verify(mockedQuery).list();
	  }
	}

}