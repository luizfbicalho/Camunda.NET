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
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using HistoricActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricActivityInstanceQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricActivityInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_ACTIVITY_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/activity-instance";
	  protected internal static readonly string HISTORIC_SINGLE_ACTIVITY_INSTANCE_URL = HISTORIC_ACTIVITY_INSTANCE_URL + "/{id}";

	  protected internal HistoryService historyServiceMock;
	  protected internal HistoricActivityInstance historicInstanceMock;
	  protected internal HistoricActivityInstanceQuery historicQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historyServiceMock = mock(typeof(HistoryService));

		// runtime service
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

		historicInstanceMock = MockProvider.createMockHistoricActivityInstance();
		historicQueryMock = mock(typeof(HistoricActivityInstanceQuery));

		when(historyServiceMock.createHistoricActivityInstanceQuery()).thenReturn(historicQueryMock);
		when(historicQueryMock.activityInstanceId(anyString())).thenReturn(historicQueryMock);
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricActivityInstance()
	  public virtual void testGetSingleHistoricActivityInstance()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_ACTIVITY_INSTANCE_URL);

		string content = response.asString();

		string returnedId = from(content).getString("id");
		string returnedParentActivityInstanceId = from(content).getString("parentActivityInstanceId");
		string returnedActivityId = from(content).getString("activityId");
		string returnedActivityName = from(content).getString("activityName");
		string returnedActivityType = from(content).getString("activityType");
		string returnedProcessDefinitionKey = from(content).getString("processDefinitionKey");
		string returnedProcessDefinitionId = from(content).getString("processDefinitionId");
		string returnedProcessInstanceId = from(content).getString("processInstanceId");
		string returnedExecutionId = from(content).getString("executionId");
		string returnedTaskId = from(content).getString("taskId");
		string returnedCalledProcessInstanceId = from(content).getString("calledProcessInstanceId");
		string returnedCalledCaseInstanceId = from(content).getString("calledCaseInstanceId");
		string returnedAssignee = from(content).getString("assignee");
		string returnedStartTime = from(content).getString("startTime");
		string returnedEndTime = from(content).getString("endTime");
		long returnedDurationInMillis = from(content).getLong("durationInMillis");
		bool canceled = from(content).getBoolean("canceled");
		bool completeScope = from(content).getBoolean("completeScope");
		string returnedTenantId = from(content).getString("tenantId");
		string returnedRemovalTime = from(content).getString("removalTime");
		string returnedRootProcessInstanceId = from(content).getString("rootProcessInstanceId");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_PARENT_ACTIVITY_INSTANCE_ID, returnedParentActivityInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_ID, returnedActivityId);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_NAME, returnedActivityName);
		Assert.assertEquals(MockProvider.EXAMPLE_ACTIVITY_TYPE, returnedActivityType);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, returnedProcessDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ID, returnedTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_PROCESS_INSTANCE_ID, returnedCalledProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_CALLED_CASE_INSTANCE_ID, returnedCalledCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME, returnedAssignee);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME, returnedStartTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_END_TIME, returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_DURATION, returnedDurationInMillis);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_CANCELED, canceled);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_IS_COMPLETE_SCOPE, completeScope);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_INSTANCE_START_TIME, returnedRemovalTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_ACTIVITY_ROOT_PROCESS_INSTANCE_ID, returnedRootProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingHistoricCaseInstance()
	  public virtual void testGetNonExistingHistoricCaseInstance()
	  {
		when(historicQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic activity instance with id '" + MockProvider.NON_EXISTING_ID + "' does not exist")).when().get(HISTORIC_SINGLE_ACTIVITY_INSTANCE_URL);
	  }

	}

}