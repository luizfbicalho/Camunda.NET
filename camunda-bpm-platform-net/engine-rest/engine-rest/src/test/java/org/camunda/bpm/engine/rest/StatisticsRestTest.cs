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
//	import static org.hamcrest.Matchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;


	using ActivityStatistics = org.camunda.bpm.engine.management.ActivityStatistics;
	using ActivityStatisticsQuery = org.camunda.bpm.engine.management.ActivityStatisticsQuery;
	using ProcessDefinitionStatistics = org.camunda.bpm.engine.management.ProcessDefinitionStatistics;
	using ProcessDefinitionStatisticsQuery = org.camunda.bpm.engine.management.ProcessDefinitionStatisticsQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;

	public class StatisticsRestTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string PROCESS_DEFINITION_STATISTICS_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition/statistics";
	  protected internal static readonly string ACTIVITY_STATISTICS_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition/{id}/statistics";
	  protected internal static readonly string ACTIVITY_STATISTICS_BY_KEY_URL = TEST_RESOURCE_ROOT_PATH + "/process-definition/key/{key}/statistics";
	  private ProcessDefinitionStatisticsQuery processDefinitionStatisticsQueryMock;
	  private ActivityStatisticsQuery activityQueryMock;
	  private ProcessDefinitionQuery processDefinitionQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		setupProcessDefinitionStatisticsMock();
		setupActivityStatisticsMock();
		setupProcessDefinitionMock();
	  }

	  private void setupActivityStatisticsMock()
	  {
		IList<ActivityStatistics> mocks = MockProvider.createMockActivityStatistics();

		activityQueryMock = mock(typeof(ActivityStatisticsQuery));
		when(activityQueryMock.list()).thenReturn(mocks);
		when(processEngine.ManagementService.createActivityStatisticsQuery(any(typeof(string)))).thenReturn(activityQueryMock);
	  }

	  private void setupProcessDefinitionStatisticsMock()
	  {
		IList<ProcessDefinitionStatistics> mocks = MockProvider.createMockProcessDefinitionStatistics();

		processDefinitionStatisticsQueryMock = mock(typeof(ProcessDefinitionStatisticsQuery));
		when(processDefinitionStatisticsQueryMock.list()).thenReturn(mocks);
		when(processEngine.ManagementService.createProcessDefinitionStatisticsQuery()).thenReturn(processDefinitionStatisticsQueryMock);
	  }

	  private void setupProcessDefinitionMock()
	  {
		ProcessDefinition mockDefinition = MockProvider.createMockDefinition();
		processDefinitionQueryMock = mock(typeof(ProcessDefinitionQuery));
		when(processDefinitionQueryMock.processDefinitionKey(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.tenantIdIn(anyString())).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.withoutTenantId()).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.latestVersion()).thenReturn(processDefinitionQueryMock);
		when(processDefinitionQueryMock.singleResult()).thenReturn(mockDefinition);
		when(processDefinitionQueryMock.list()).thenReturn(Collections.singletonList(mockDefinition));
		when(processDefinitionQueryMock.count()).thenReturn(1L);
		when(processEngine.RepositoryService.createProcessDefinitionQuery()).thenReturn(processDefinitionQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStatisticsRetrievalPerProcessDefinitionVersion()
	  public virtual void testStatisticsRetrievalPerProcessDefinitionVersion()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("definition.size()", @is(2)).body("definition.id", hasItems(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, MockProvider.ANOTHER_EXAMPLE_PROCESS_DEFINITION_ID)).when().get(PROCESS_DEFINITION_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionStatisticsRetrieval()
	  public virtual void testProcessDefinitionStatisticsRetrieval()
	  {
		given().then().expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("[0].definition.id", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("[0].definition.key", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("[0].definition.category", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY)).body("[0].definition.name", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME)).body("[0].definition.description", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION)).body("[0].definition.deploymentId", equalTo(MockProvider.EXAMPLE_DEPLOYMENT_ID)).body("[0].definition.version", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION)).body("[0].definition.resource", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME)).body("[0].definition.diagram", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME)).body("[0].definition.tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("[0].definition.versionTag", equalTo(MockProvider.EXAMPLE_VERSION_TAG)).when().get(PROCESS_DEFINITION_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsRetrieval()
	  public virtual void testActivityStatisticsRetrieval()
	  {
		given().pathParam("id", "aDefinitionId").then().expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("id", hasItems(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID)).when().get(ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsRetrievalByKey()
	  public virtual void testActivityStatisticsRetrievalByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", @is(2)).body("id", hasItems(MockProvider.EXAMPLE_ACTIVITY_ID, MockProvider.ANOTHER_EXAMPLE_ACTIVITY_ID)).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalFailedJobsOption()
	  public virtual void testAdditionalFailedJobsOption()
	  {
		given().queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeFailedJobs();
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalIncidentsOption()
	  public virtual void testAdditionalIncidentsOption()
	  {
		given().queryParam("incidents", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeIncidents();
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalIncidentsForTypeOption()
	  public virtual void testAdditionalIncidentsForTypeOption()
	  {
		given().queryParam("incidentsForType", "failedJob").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalIncidentsAndFailedJobsOption()
	  public virtual void testAdditionalIncidentsAndFailedJobsOption()
	  {
		given().queryParam("incidents", "true").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeFailedJobs();
		inOrder.verify(processDefinitionStatisticsQueryMock).includeIncidents();
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalIncidentsForTypeAndFailedJobsOption()
	  public virtual void testAdditionalIncidentsForTypeAndFailedJobsOption()
	  {
		given().queryParam("incidentsForType", "failedJob").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeFailedJobs();
		inOrder.verify(processDefinitionStatisticsQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalIncidentsAndIncidentsForType()
	  public virtual void testAdditionalIncidentsAndIncidentsForType()
	  {
		given().queryParam("incidents", "true").queryParam("incidentsForType", "anIncidentTpye").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsWithFailedJobs()
	  public virtual void testActivityStatisticsWithFailedJobs()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsWithFailedJobsByKey()
	  public virtual void testActivityStatisticsWithFailedJobsByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsWithIncidents()
	  public virtual void testActivityStatisticsWithIncidents()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("incidents", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeIncidents();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsWithIncidentsByKey()
	  public virtual void testActivityStatisticsWithIncidentsByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("incidents", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeIncidents();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsIncidentsForTypeTypeOption()
	  public virtual void testActivityStatisticsIncidentsForTypeTypeOption()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("incidentsForType", "failedJob").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivityStatisticsIncidentsForTypeTypeOptionByKey()
	  public virtual void testActivityStatisticsIncidentsForTypeTypeOptionByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("incidentsForType", "failedJob").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsAndFailedJobsOption()
	  public virtual void testActivtyStatisticsIncidentsAndFailedJobsOption()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("incidents", "true").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).includeIncidents();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsAndFailedJobsOptionByKey()
	  public virtual void testActivtyStatisticsIncidentsAndFailedJobsOptionByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("incidents", "true").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).includeIncidents();
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsForTypeAndFailedJobsOption()
	  public virtual void testActivtyStatisticsIncidentsForTypeAndFailedJobsOption()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("incidentsForType", "failedJob").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsByIdThrowsAuthorizationException()
	  public virtual void testActivtyStatisticsByIdThrowsAuthorizationException()
	  {
		string message = "expected exception";
		when(activityQueryMock.list()).thenThrow(new AuthorizationException(message));

		given().pathParam("id", "aDefinitionId").then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsForTypeAndFailedJobsOptionByKey()
	  public virtual void testActivtyStatisticsIncidentsForTypeAndFailedJobsOptionByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("incidentsForType", "failedJob").queryParam("failedJobs", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);

		InOrder inOrder = Mockito.inOrder(activityQueryMock);
		inOrder.verify(activityQueryMock).includeFailedJobs();
		inOrder.verify(activityQueryMock).includeIncidentsForType("failedJob");
		inOrder.verify(activityQueryMock).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsAndIncidentsForType()
	  public virtual void testActivtyStatisticsIncidentsAndIncidentsForType()
	  {
		given().pathParam("id", "aDefinitionId").queryParam("incidents", "true").queryParam("incidentsForType", "anIncidentTpye").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(ACTIVITY_STATISTICS_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsIncidentsAndIncidentsForTypeByKey()
	  public virtual void testActivtyStatisticsIncidentsAndIncidentsForTypeByKey()
	  {
		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).queryParam("incidents", "true").queryParam("incidentsForType", "anIncidentTpye").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testActivtyStatisticsByIdThrowsAuthorizationExceptionByKey()
	  public virtual void testActivtyStatisticsByIdThrowsAuthorizationExceptionByKey()
	  {
		string message = "expected exception";
		when(activityQueryMock.list()).thenThrow(new AuthorizationException(message));

		given().pathParam("key", MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY).then().expect().statusCode(Status.FORBIDDEN.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(AuthorizationException).Name)).body("message", equalTo(message)).when().get(ACTIVITY_STATISTICS_BY_KEY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessDefinitionStatisticsWithRootIncidents()
	  public virtual void testProcessDefinitionStatisticsWithRootIncidents()
	  {
		given().queryParam("rootIncidents", "true").then().expect().statusCode(Status.OK.StatusCode).when().get(PROCESS_DEFINITION_STATISTICS_URL);

		InOrder inOrder = Mockito.inOrder(processDefinitionStatisticsQueryMock);
		inOrder.verify(processDefinitionStatisticsQueryMock).includeRootIncidents();
		inOrder.verify(processDefinitionStatisticsQueryMock).list();
	  }
	}

}