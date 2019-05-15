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

	using HistoricCaseActivityInstance = org.camunda.bpm.engine.history.HistoricCaseActivityInstance;
	using HistoricCaseActivityInstanceQuery = org.camunda.bpm.engine.history.HistoricCaseActivityInstanceQuery;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class HistoricCaseActivityInstanceRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string HISTORIC_CASE_ACTIVITY_INSTANCE_URL = TEST_RESOURCE_ROOT_PATH + "/history/case-activity-instance";
	  protected internal static readonly string HISTORIC_SINGLE_CASE_ACTIVITY_INSTANCE_URL = HISTORIC_CASE_ACTIVITY_INSTANCE_URL + "/{id}";

	  protected internal HistoryService historyServiceMock;
	  protected internal HistoricCaseActivityInstance historicInstanceMock;
	  protected internal HistoricCaseActivityInstanceQuery historicQueryMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		historyServiceMock = mock(typeof(HistoryService));

		// runtime service
		when(processEngine.HistoryService).thenReturn(historyServiceMock);

		historicInstanceMock = MockProvider.createMockHistoricCaseActivityInstance();
		historicQueryMock = mock(typeof(HistoricCaseActivityInstanceQuery));

		when(historyServiceMock.createHistoricCaseActivityInstanceQuery()).thenReturn(historicQueryMock);
		when(historicQueryMock.caseActivityInstanceId(anyString())).thenReturn(historicQueryMock);
		when(historicQueryMock.singleResult()).thenReturn(historicInstanceMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetSingleHistoricCaseInstance()
	  public virtual void testGetSingleHistoricCaseInstance()
	  {
		Response response = given().pathParam("id", MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(HISTORIC_SINGLE_CASE_ACTIVITY_INSTANCE_URL);

		string content = response.asString();

		string returnedCaseActivityInstanceId = from(content).getString("id");
		string returnedParentCaseActivityInstanceId = from(content).getString("parentCaseActivityInstanceId");
		string returnedCaseActivityId = from(content).getString("caseActivityId");
		string returnedCaseActivityName = from(content).getString("caseActivityName");
		string returnedCaseActivityType = from(content).getString("caseActivityType");
		string returnedCaseDefinitionId = from(content).getString("caseDefinitionId");
		string returnedCaseInstanceId = from(content).getString("caseInstanceId");
		string returnedCaseExecutionId = from(content).getString("caseExecutionId");
		string returnedTaskId = from(content).getString("taskId");
		string returnedCalledProcessInstanceId = from(content).getString("calledProcessInstanceId");
		string returnedCalledCaseInstanceId = from(content).getString("calledCaseInstanceId");
		string returnedCreateTime = from(content).getString("createTime");
		string returnedEndTime = from(content).getString("endTime");
		string returnedTenantId = from(content).getString("tenantId");
		long returnedDurationInMillis = from(content).getLong("durationInMillis");
		bool required = from(content).getBoolean("required");
		bool available = from(content).getBoolean("available");
		bool enabled = from(content).getBoolean("enabled");
		bool disabled = from(content).getBoolean("disabled");
		bool active = from(content).getBoolean("active");
		bool completed = from(content).getBoolean("completed");
		bool terminated = from(content).getBoolean("terminated");

		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_ID, returnedCaseActivityInstanceId);
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
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_CREATE_TIME, returnedCreateTime);
		Assert.assertEquals(MockProvider.EXAMPLE_HISTORIC_CASE_ACTIVITY_INSTANCE_END_TIME, returnedEndTime);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);
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
//ORIGINAL LINE: @Test public void testGetNonExistingHistoricCaseInstance()
	  public virtual void testGetNonExistingHistoricCaseInstance()
	  {
		when(historicQueryMock.singleResult()).thenReturn(null);

		given().pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Historic case activity instance with id '" + MockProvider.NON_EXISTING_ID + "' does not exist")).when().get(HISTORIC_SINGLE_CASE_ACTIVITY_INSTANCE_URL);
	  }

	}

}