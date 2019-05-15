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
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_JOB_DEFINITION_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_TENANT_ID_LIST;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.NON_EXISTING_JOB_DEFINITION_ID;
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


	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;


	/// 
	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class IncidentRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string INCIDENT_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/incident";
	  protected internal static readonly string INCIDENT_COUNT_QUERY_URL = INCIDENT_QUERY_URL + "/count";

	  private IncidentQuery mockedQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		IList<Incident> incidents = MockProvider.createMockIncidents();

		mockedQuery = setupMockIncidentQuery(incidents);
	  }

	  private IncidentQuery setupMockIncidentQuery(IList<Incident> incidents)
	  {
		IncidentQuery sampleQuery = mock(typeof(IncidentQuery));

		when(sampleQuery.list()).thenReturn(incidents);
		when(sampleQuery.count()).thenReturn((long) incidents.Count);

		when(processEngine.RuntimeService.createIncidentQuery()).thenReturn(sampleQuery);

		return sampleQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("processInstanceId", queryKey).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

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
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).then().expect().statusCode(expectedStatus.StatusCode).when().get(INCIDENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(INCIDENT_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentMessage", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentMessage();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentMessage", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentMessage();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentTimestamp", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentTimestamp();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentTimestamp", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentTimestamp();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentType", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentType();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("incidentType", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentType();
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
		executeAndVerifySorting("activityId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("activityId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByActivityId();
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
		executeAndVerifySorting("causeIncidentId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByCauseIncidentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("causeIncidentId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByCauseIncidentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("rootCauseIncidentId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByRootCauseIncidentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("rootCauseIncidentId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByRootCauseIncidentId();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("configuration", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByConfiguration();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("configuration", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByConfiguration();
		inOrder.verify(mockedQuery).desc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentId();
		inOrder.verify(mockedQuery).asc();

		inOrder = Mockito.inOrder(mockedQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockedQuery).orderByIncidentId();
		inOrder.verify(mockedQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {
		int firstResult = 0;
		int maxResults = 10;

		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingFirstResultParameter()
	  public virtual void testMissingFirstResultParameter()
	  {
		int maxResults = 10;

		given().queryParam("maxResults", maxResults).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).listPage(0, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMissingMaxResultsParameter()
	  public virtual void testMissingMaxResultsParameter()
	  {
		int firstResult = 10;

		given().queryParam("firstResult", firstResult).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).listPage(firstResult, int.MaxValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(INCIDENT_COUNT_QUERY_URL);

		verify(mockedQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHistoricTaskInstanceQuery()
	  public virtual void testSimpleHistoricTaskInstanceQuery()
	  {
		Response response = given().then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		InOrder inOrder = inOrder(mockedQuery);
		inOrder.verify(mockedQuery).list();

		string content = response.asString();
		IList<string> incidents = from(content).getList("");
		Assert.assertEquals("There should be one incident returned.", 1, incidents.Count);
		Assert.assertNotNull("The returned incident should not be null.", incidents[0]);

		string returnedId = from(content).getString("[0].id");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedExecutionId = from(content).getString("[0].executionId");
		DateTime returnedIncidentTimestamp = DateTimeUtil.parseDate(from(content).getString("[0].incidentTimestamp"));
		string returnedIncidentType = from(content).getString("[0].incidentType");
		string returnedActivityId = from(content).getString("[0].activityId");
		string returnedCauseIncidentId = from(content).getString("[0].causeIncidentId");
		string returnedRootCauseIncidentId = from(content).getString("[0].rootCauseIncidentId");
		string returnedConfiguration = from(content).getString("[0].configuration");
		string returnedIncidentMessage = from(content).getString("[0].incidentMessage");
		string returnedTenantId = from(content).getString("[0].tenantId");
		string returnedJobDefinitionId = from(content).getString("[0].jobDefinitionId");

		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_PROC_INST_ID, returnedProcessInstanceId);
		Assert.assertEquals(DateTimeUtil.parseDate(MockProvider.EXAMPLE_INCIDENT_TIMESTAMP), returnedIncidentTimestamp);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_PROC_DEF_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_TYPE, returnedIncidentType);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_ACTIVITY_ID, returnedActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_CAUSE_INCIDENT_ID, returnedCauseIncidentId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_ROOT_CAUSE_INCIDENT_ID, returnedRootCauseIncidentId);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_CONFIGURATION, returnedConfiguration);
		Assert.assertEquals(MockProvider.EXAMPLE_INCIDENT_MESSAGE, returnedIncidentMessage);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_JOB_DEFINITION_ID, returnedJobDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentId()
	  public virtual void testQueryByIncidentId()
	  {
		string incidentId = MockProvider.EXAMPLE_INCIDENT_ID;

		given().queryParam("incidentId", incidentId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).incidentId(incidentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentType()
	  public virtual void testQueryByIncidentType()
	  {
		string incidentType = MockProvider.EXAMPLE_INCIDENT_TYPE;

		given().queryParam("incidentType", incidentType).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).incidentType(incidentType);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByIncidentMessage()
	  public virtual void testQueryByIncidentMessage()
	  {
		string incidentMessage = MockProvider.EXAMPLE_INCIDENT_MESSAGE;

		given().queryParam("incidentMessage", incidentMessage).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).incidentMessage(incidentMessage);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessDefinitionId()
	  public virtual void testQueryByProcessDefinitionId()
	  {
		string processDefinitionId = MockProvider.EXAMPLE_INCIDENT_PROC_DEF_ID;

		given().queryParam("processDefinitionId", processDefinitionId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).processDefinitionId(processDefinitionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByProcessInstanceId()
	  public virtual void testQueryByProcessInstanceId()
	  {
		string processInstanceId = MockProvider.EXAMPLE_INCIDENT_PROC_INST_ID;

		given().queryParam("processInstanceId", processInstanceId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).processInstanceId(processInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByExecutionId()
	  public virtual void testQueryByExecutionId()
	  {
		string executionId = MockProvider.EXAMPLE_INCIDENT_EXECUTION_ID;

		given().queryParam("executionId", executionId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).executionId(executionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByActivityId()
	  public virtual void testQueryByActivityId()
	  {
		string activityId = MockProvider.EXAMPLE_INCIDENT_ACTIVITY_ID;

		given().queryParam("activityId", activityId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).activityId(activityId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByCauseIncidentId()
	  public virtual void testQueryByCauseIncidentId()
	  {
		string causeIncidentId = MockProvider.EXAMPLE_INCIDENT_CAUSE_INCIDENT_ID;

		given().queryParam("causeIncidentId", causeIncidentId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).causeIncidentId(causeIncidentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByRootCauseIncidentId()
	  public virtual void testQueryByRootCauseIncidentId()
	  {
		string rootCauseIncidentId = MockProvider.EXAMPLE_INCIDENT_ROOT_CAUSE_INCIDENT_ID;

		given().queryParam("rootCauseIncidentId", rootCauseIncidentId).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).rootCauseIncidentId(rootCauseIncidentId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByConfiguration()
	  public virtual void testQueryByConfiguration()
	  {
		string configuration = MockProvider.EXAMPLE_INCIDENT_CONFIGURATION;

		given().queryParam("configuration", configuration).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).configuration(configuration);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryByTenantIds()
	  public virtual void testQueryByTenantIds()
	  {
		mockedQuery = setupMockIncidentQuery(Arrays.asList(MockProvider.createMockIncident(MockProvider.EXAMPLE_TENANT_ID), MockProvider.createMockIncident(MockProvider.ANOTHER_EXAMPLE_TENANT_ID)));

		Response response = given().queryParam("tenantIdIn", EXAMPLE_TENANT_ID_LIST).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

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
//ORIGINAL LINE: @Test public void testQueryByJobDefinitionIds()
	  public virtual void testQueryByJobDefinitionIds()
	  {
		string jobDefinitionIds = EXAMPLE_JOB_DEFINITION_ID + "," + NON_EXISTING_JOB_DEFINITION_ID;

		given().queryParam("jobDefinitionIdIn", jobDefinitionIds).then().expect().statusCode(Status.OK.StatusCode).when().get(INCIDENT_QUERY_URL);

		verify(mockedQuery).jobDefinitionIdIn(EXAMPLE_JOB_DEFINITION_ID, NON_EXISTING_JOB_DEFINITION_ID);
		verify(mockedQuery).list();
	  }

	}

}