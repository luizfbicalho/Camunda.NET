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
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.QueryParamUtils.arrayAsCommaSeperatedList;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.reset;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using ProcessApplicationInfo = org.camunda.bpm.application.ProcessApplicationInfo;
	using RuntimeContainerDelegate = org.camunda.bpm.container.RuntimeContainerDelegate;
	using User = org.camunda.bpm.engine.identity.User;
	using UserQuery = org.camunda.bpm.engine.identity.UserQuery;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using CaseDefinition = org.camunda.bpm.engine.repository.CaseDefinition;
	using CaseDefinitionQuery = org.camunda.bpm.engine.repository.CaseDefinitionQuery;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessDefinitionQuery = org.camunda.bpm.engine.repository.ProcessDefinitionQuery;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using Hal = org.camunda.bpm.engine.rest.hal.Hal;
	using EqualsList = org.camunda.bpm.engine.rest.helper.EqualsList;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using ValueGenerator = org.camunda.bpm.engine.rest.helper.ValueGenerator;
	using EqualsPrimitiveValue = org.camunda.bpm.engine.rest.helper.variable.EqualsPrimitiveValue;
	using OrderingBuilder = org.camunda.bpm.engine.rest.util.OrderingBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using DelegationState = org.camunda.bpm.engine.task.DelegationState;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;

	using ContentType = io.restassured.http.ContentType;
	using Response = io.restassured.response.Response;

	public class TaskRestServiceQueryTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  protected internal static readonly string TASK_QUERY_URL = TEST_RESOURCE_ROOT_PATH + "/task";
	  protected internal static readonly string TASK_COUNT_QUERY_URL = TASK_QUERY_URL + "/count";

	  private const string SAMPLE_VAR_NAME = "varName";
	  private const string SAMPLE_VAR_VALUE = "varValue";

	  private TaskQuery mockQuery;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		mockQuery = setUpMockTaskQuery(MockProvider.createMockTasks());
	  }

	  private TaskQuery setUpMockTaskQuery(IList<Task> mockedTasks)
	  {
		TaskQuery sampleTaskQuery = mock(typeof(TaskQueryImpl));
		when(sampleTaskQuery.list()).thenReturn(mockedTasks);
		when(sampleTaskQuery.count()).thenReturn((long) mockedTasks.Count);
		when(sampleTaskQuery.taskCandidateGroup(anyString())).thenReturn(sampleTaskQuery);

		when(processEngine.TaskService.createTaskQuery()).thenReturn(sampleTaskQuery);

		return sampleTaskQuery;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyQuery()
	  public virtual void testEmptyQuery()
	  {
		string queryKey = "";
		given().queryParam("name", queryKey).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidDateParameter()
	  public virtual void testInvalidDateParameter()
	  {
		given().queryParams("due", "anInvalidDate").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Cannot set query parameter 'due' to value 'anInvalidDate': " + "Cannot convert value \"anInvalidDate\" to java type java.util.Date")).when().get(TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortByParameterOnly()
	  public virtual void testSortByParameterOnly()
	  {
		given().queryParam("sortBy", "dueDate").header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortOrderParameterOnly()
	  public virtual void testSortOrderParameterOnly()
	  {
		given().queryParam("sortOrder", "asc").then().expect().statusCode(Status.BAD_REQUEST.StatusCode).contentType(ContentType.JSON).body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", equalTo("Only a single sorting parameter specified. sortBy and sortOrder required")).when().get(TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleTaskQuery()
	  public virtual void testSimpleTaskQuery()
	  {
		string queryName = "name";

		Response response = given().queryParam("name", queryName).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).taskName(queryName);
		inOrder.verify(mockQuery).list();

		string content = response.asString();
		IList<string> instances = from(content).getList("");
		Assert.assertEquals("There should be one task returned.", 1, instances.Count);
		Assert.assertNotNull("The returned task should not be null.", instances[0]);

		string returnedTaskName = from(content).getString("[0].name");
		string returnedId = from(content).getString("[0].id");
		string returendAssignee = from(content).getString("[0].assignee");
		string returnedCreateTime = from(content).getString("[0].created");
		string returnedDueDate = from(content).getString("[0].due");
		string returnedFollowUpDate = from(content).getString("[0].followUp");
		string returnedDelegationState = from(content).getString("[0].delegationState");
		string returnedDescription = from(content).getString("[0].description");
		string returnedExecutionId = from(content).getString("[0].executionId");
		string returnedOwner = from(content).getString("[0].owner");
		string returnedParentTaskId = from(content).getString("[0].parentTaskId");
		int returnedPriority = from(content).getInt("[0].priority");
		string returnedProcessDefinitionId = from(content).getString("[0].processDefinitionId");
		string returnedProcessInstanceId = from(content).getString("[0].processInstanceId");
		string returnedTaskDefinitionKey = from(content).getString("[0].taskDefinitionKey");
		string returnedCaseDefinitionId = from(content).getString("[0].caseDefinitionId");
		string returnedCaseInstanceId = from(content).getString("[0].caseInstanceId");
		string returnedCaseExecutionId = from(content).getString("[0].caseExecutionId");
		bool returnedSuspensionState = from(content).getBoolean("[0].suspended");
		string returnedFormKey = from(content).getString("[0].formKey");
		string returnedTenantId = from(content).getString("[0].tenantId");

		Assert.assertEquals(MockProvider.EXAMPLE_TASK_NAME, returnedTaskName);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME, returendAssignee);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_CREATE_TIME, returnedCreateTime);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DUE_DATE, returnedDueDate);
		Assert.assertEquals(MockProvider.EXAMPLE_FOLLOW_UP_DATE, returnedFollowUpDate);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DELEGATION_STATE.ToString(), returnedDelegationState);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DESCRIPTION, returnedDescription);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_OWNER, returnedOwner);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID, returnedParentTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_PRIORITY, returnedPriority);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DEFINITION_KEY, returnedTaskDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_EXECUTION_ID, returnedCaseExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_SUSPENSION_STATE, returnedSuspensionState);
		Assert.assertEquals(MockProvider.EXAMPLE_FORM_KEY, returnedFormKey);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSimpleHalTaskQuery()
	  public virtual void testSimpleHalTaskQuery()
	  {
		string queryName = "name";

		// setup user query mock
		IList<User> mockUsers = MockProvider.createMockUsers();
		UserQuery sampleUserQuery = mock(typeof(UserQuery));
		when(sampleUserQuery.listPage(0, 1)).thenReturn(mockUsers);
		when(sampleUserQuery.userIdIn(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.userIdIn(MockProvider.EXAMPLE_TASK_OWNER)).thenReturn(sampleUserQuery);
		when(sampleUserQuery.count()).thenReturn(1l);
		when(processEngine.IdentityService.createUserQuery()).thenReturn(sampleUserQuery);

		// setup process definition query mock
		IList<ProcessDefinition> mockDefinitions = MockProvider.createMockDefinitions();
		ProcessDefinitionQuery sampleProcessDefinitionQuery = mock(typeof(ProcessDefinitionQuery));
		when(sampleProcessDefinitionQuery.listPage(0, 1)).thenReturn(mockDefinitions);
		when(sampleProcessDefinitionQuery.processDefinitionIdIn(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).thenReturn(sampleProcessDefinitionQuery);
		when(sampleProcessDefinitionQuery.count()).thenReturn(1l);
		when(processEngine.RepositoryService.createProcessDefinitionQuery()).thenReturn(sampleProcessDefinitionQuery);

		// setup case definition query mock
		IList<CaseDefinition> mockCaseDefinitions = MockProvider.createMockCaseDefinitions();
		CaseDefinitionQuery sampleCaseDefinitionQuery = mock(typeof(CaseDefinitionQuery));
		when(sampleCaseDefinitionQuery.listPage(0, 1)).thenReturn(mockCaseDefinitions);
		when(sampleCaseDefinitionQuery.caseDefinitionIdIn(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).thenReturn(sampleCaseDefinitionQuery);
		when(sampleCaseDefinitionQuery.count()).thenReturn(1l);
		when(processEngine.RepositoryService.createCaseDefinitionQuery()).thenReturn(sampleCaseDefinitionQuery);

		// setup example process application context path
		when(processEngine.ManagementService.getProcessApplicationForDeployment(MockProvider.EXAMPLE_DEPLOYMENT_ID)).thenReturn(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME);

		// replace the runtime container delegate & process application service with a mock
		ProcessApplicationService processApplicationService = mock(typeof(ProcessApplicationService));
		ProcessApplicationInfo appMock = MockProvider.createMockProcessApplicationInfo();
		when(processApplicationService.getProcessApplicationInfo(MockProvider.EXAMPLE_PROCESS_APPLICATION_NAME)).thenReturn(appMock);

		RuntimeContainerDelegate @delegate = mock(typeof(RuntimeContainerDelegate));
		when(@delegate.ProcessApplicationService).thenReturn(processApplicationService);
		org.camunda.bpm.container.RuntimeContainerDelegate_Fields.INSTANCE.set(@delegate);

		Response response = given().queryParam("name", queryName).header("accept", Hal.APPLICATION_HAL_JSON).then().expect().statusCode(Status.OK.StatusCode).contentType(Hal.APPLICATION_HAL_JSON).when().get(TASK_QUERY_URL);

		InOrder inOrder = inOrder(mockQuery);
		inOrder.verify(mockQuery).taskName(queryName);
		inOrder.verify(mockQuery).list();

		// validate embedded tasks
		string content = response.asString();
		IList<IDictionary<string, object>> instances = from(content).getList("_embedded.task");
		Assert.assertEquals("There should be one task returned.", 1, instances.Count);
		Assert.assertNotNull("The returned task should not be null.", instances[0]);

		IDictionary<string, object> taskObject = instances[0];

		string returnedTaskName = (string) taskObject["name"];
		string returnedId = (string) taskObject["id"];
		string returnedAssignee = (string) taskObject["assignee"];
		string returnedCreateTime = (string) taskObject["created"];
		string returnedDueDate = (string) taskObject["due"];
		string returnedFollowUpDate = (string) taskObject["followUp"];
		string returnedDelegationState = (string) taskObject["delegationState"];
		string returnedDescription = (string) taskObject["description"];
		string returnedExecutionId = (string) taskObject["executionId"];
		string returnedOwner = (string) taskObject["owner"];
		string returnedParentTaskId = (string) taskObject["parentTaskId"];
		int returnedPriority = (int?) taskObject["priority"].Value;
		string returnedProcessDefinitionId = (string) taskObject["processDefinitionId"];
		string returnedProcessInstanceId = (string) taskObject["processInstanceId"];
		string returnedTaskDefinitionKey = (string) taskObject["taskDefinitionKey"];
		string returnedCaseDefinitionId = (string) taskObject["caseDefinitionId"];
		string returnedCaseInstanceId = (string) taskObject["caseInstanceId"];
		string returnedCaseExecutionId = (string) taskObject["caseExecutionId"];
		bool returnedSuspensionState = (bool?) taskObject["suspended"].Value;
		string returnedFormKey = (string) taskObject["formKey"];
		string returnedTenantId = (string) taskObject["tenantId"];

		Assert.assertEquals(MockProvider.EXAMPLE_TASK_NAME, returnedTaskName);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ID, returnedId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME, returnedAssignee);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_CREATE_TIME, returnedCreateTime);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DUE_DATE, returnedDueDate);
		Assert.assertEquals(MockProvider.EXAMPLE_FOLLOW_UP_DATE, returnedFollowUpDate);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DELEGATION_STATE.ToString(), returnedDelegationState);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DESCRIPTION, returnedDescription);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_EXECUTION_ID, returnedExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_OWNER, returnedOwner);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID, returnedParentTaskId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_PRIORITY, returnedPriority);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, returnedProcessDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID, returnedProcessInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_DEFINITION_KEY, returnedTaskDefinitionKey);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, returnedCaseDefinitionId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_INSTANCE_ID, returnedCaseInstanceId);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_EXECUTION_ID, returnedCaseExecutionId);
		Assert.assertEquals(MockProvider.EXAMPLE_TASK_SUSPENSION_STATE, returnedSuspensionState);
		Assert.assertEquals(MockProvider.EXAMPLE_TENANT_ID, returnedTenantId);

		// validate the task count
		Assert.assertEquals(1l, from(content).getLong("count"));

		// validate links
		IDictionary<string, object> selfReference = from(content).getMap("_links.self");
		Assert.assertNotNull(selfReference);
		Assert.assertEquals("/task", selfReference["href"]);

		// validate embedded assignees:
		IList<IDictionary<string, object>> embeddedAssignees = from(content).getList("_embedded.assignee");
		Assert.assertEquals("There should be one assignee returned.", 1, embeddedAssignees.Count);
		IDictionary<string, object> embeddedAssignee = embeddedAssignees[0];
		Assert.assertNotNull("The returned assignee should not be null.", embeddedAssignee);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_ID, embeddedAssignee["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_FIRST_NAME, embeddedAssignee["firstName"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_LAST_NAME, embeddedAssignee["lastName"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_EMAIL, embeddedAssignee["email"]);

		// validate embedded owners:
		IList<IDictionary<string, object>> embeddedOwners = from(content).getList("_embedded.owner");
		Assert.assertEquals("There should be one owner returned.", 1, embeddedOwners.Count);
		IDictionary<string, object> embeddedOwner = embeddedOwners[0];
		Assert.assertNotNull("The returned owner should not be null.", embeddedOwner);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_ID, embeddedOwner["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_FIRST_NAME, embeddedOwner["firstName"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_LAST_NAME, embeddedOwner["lastName"]);
		Assert.assertEquals(MockProvider.EXAMPLE_USER_EMAIL, embeddedOwner["email"]);

		// validate embedded processDefinitions:
		IList<IDictionary<string, object>> embeddedDefinitions = from(content).getList("_embedded.processDefinition");
		Assert.assertEquals("There should be one processDefinition returned.", 1, embeddedDefinitions.Count);
		IDictionary<string, object> embeddedProcessDefinition = embeddedDefinitions[0];
		Assert.assertNotNull("The returned processDefinition should not be null.", embeddedProcessDefinition);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID, embeddedProcessDefinition["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, embeddedProcessDefinition["key"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_CATEGORY, embeddedProcessDefinition["category"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME, embeddedProcessDefinition["name"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DESCRIPTION, embeddedProcessDefinition["description"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_VERSION, embeddedProcessDefinition["version"]);
		Assert.assertEquals(MockProvider.EXAMPLE_VERSION_TAG, embeddedProcessDefinition["versionTag"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_RESOURCE_NAME, embeddedProcessDefinition["resource"]);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, embeddedProcessDefinition["deploymentId"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_DIAGRAM_RESOURCE_NAME, embeddedProcessDefinition["diagram"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_DEFINITION_IS_SUSPENDED, embeddedProcessDefinition["suspended"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH, embeddedProcessDefinition["contextPath"]);

		// validate embedded caseDefinitions:
		IList<IDictionary<string, object>> embeddedCaseDefinitions = from(content).getList("_embedded.caseDefinition");
		Assert.assertEquals("There should be one caseDefinition returned.", 1, embeddedCaseDefinitions.Count);
		IDictionary<string, object> embeddedCaseDefinition = embeddedCaseDefinitions[0];
		Assert.assertNotNull("The returned caseDefinition should not be null.", embeddedCaseDefinition);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_ID, embeddedCaseDefinition["id"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_KEY, embeddedCaseDefinition["key"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_CATEGORY, embeddedCaseDefinition["category"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_NAME, embeddedCaseDefinition["name"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_VERSION, embeddedCaseDefinition["version"]);
		Assert.assertEquals(MockProvider.EXAMPLE_CASE_DEFINITION_RESOURCE_NAME, embeddedCaseDefinition["resource"]);
		Assert.assertEquals(MockProvider.EXAMPLE_DEPLOYMENT_ID, embeddedCaseDefinition["deploymentId"]);
		Assert.assertEquals(MockProvider.EXAMPLE_PROCESS_APPLICATION_CONTEXT_PATH, embeddedCaseDefinition["contextPath"]);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNoParametersQuery()
	  public virtual void testNoParametersQuery()
	  {
		given().header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).initializeFormKeys();
		verify(mockQuery).list();
		verifyNoMoreInteractions(mockQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAdditionalParametersExcludingVariables()
	  public virtual void testAdditionalParametersExcludingVariables()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;
		IDictionary<string, int> intQueryParameters = CompleteIntQueryParameters;
		IDictionary<string, bool> booleanQueryParameters = CompleteBooleanQueryParameters;

		IDictionary<string, String[]> arrayQueryParameters = CompleteStringArrayQueryParameters;

		given().queryParams(stringQueryParameters).queryParams(intQueryParameters).queryParams(booleanQueryParameters).queryParam("activityInstanceIdIn", arrayAsCommaSeperatedList(arrayQueryParameters["activityInstanceIdIn"])).queryParam("taskDefinitionKeyIn", arrayAsCommaSeperatedList(arrayQueryParameters["taskDefinitionKeyIn"])).queryParam("processDefinitionKeyIn", arrayAsCommaSeperatedList(arrayQueryParameters["processDefinitionKeyIn"])).queryParam("processInstanceBusinessKeyIn", arrayAsCommaSeperatedList(arrayQueryParameters["processInstanceBusinessKeyIn"])).queryParam("tenantIdIn", arrayAsCommaSeperatedList(arrayQueryParameters["tenantIdIn"])).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verifyIntegerParameterQueryInvocations();
		verifyStringParameterQueryInvocations();
		verifyBooleanParameterQueryInvocation();
		verifyStringArrayParametersInvocations();

		verify(mockQuery).list();
	  }

	  private void verifyIntegerParameterQueryInvocations()
	  {
		IDictionary<string, int> intQueryParameters = CompleteIntQueryParameters;

		verify(mockQuery).taskMaxPriority(intQueryParameters["maxPriority"]);
		verify(mockQuery).taskMinPriority(intQueryParameters["minPriority"]);
		verify(mockQuery).taskPriority(intQueryParameters["priority"]);
	  }

	  private IDictionary<string, int> CompleteIntQueryParameters
	  {
		  get
		  {
			IDictionary<string, int> parameters = new Dictionary<string, int>();
    
			parameters["maxPriority"] = 10;
			parameters["minPriority"] = 9;
			parameters["priority"] = 8;
    
			return parameters;
		  }
	  }

	  private IDictionary<string, String[]> CompleteStringArrayQueryParameters
	  {
		  get
		  {
			IDictionary<string, String[]> parameters = new Dictionary<string, String[]>();
    
			string[] activityInstanceIds = new string[] {"anActivityInstanceId", "anotherActivityInstanceId"};
			string[] taskDefinitionKeys = new string[] {"aTaskDefinitionKey", "anotherTaskDefinitionKey"};
			string[] processDefinitionKeys = new string[] {"aProcessDefinitionKey", "anotherProcessDefinitionKey"};
			string[] processInstanceBusinessKeys = new string[] {"aBusinessKey", "anotherBusinessKey"};
			string[] tenantIds = new string[] {MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID};
    
    
			parameters["activityInstanceIdIn"] = activityInstanceIds;
			parameters["taskDefinitionKeyIn"] = taskDefinitionKeys;
			parameters["processDefinitionKeyIn"] = processDefinitionKeys;
			parameters["processInstanceBusinessKeyIn"] = processInstanceBusinessKeys;
			parameters["tenantIdIn"] = tenantIds;
    
			return parameters;
		  }
	  }

	  private IDictionary<string, string> CompleteStringQueryParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
    
			parameters["processInstanceBusinessKey"] = "aBusinessKey";
			parameters["processInstanceBusinessKeyLike"] = "aBusinessKeyLike";
			parameters["processDefinitionKey"] = "aProcDefKey";
			parameters["processDefinitionId"] = "aProcDefId";
			parameters["executionId"] = "anExecId";
			parameters["processDefinitionName"] = "aProcDefName";
			parameters["processDefinitionNameLike"] = "aProcDefNameLike";
			parameters["processInstanceId"] = "aProcInstId";
			parameters["assignee"] = "anAssignee";
			parameters["assigneeLike"] = "anAssigneeLike";
			parameters["candidateGroup"] = "aCandidateGroup";
			parameters["candidateUser"] = "aCandidate";
			parameters["includeAssignedTasks"] = "false";
			parameters["taskDefinitionKey"] = "aTaskDefKey";
			parameters["taskDefinitionKeyLike"] = "aTaskDefKeyLike";
			parameters["description"] = "aDesc";
			parameters["descriptionLike"] = "aDescLike";
			parameters["involvedUser"] = "anInvolvedPerson";
			parameters["name"] = "aName";
			parameters["nameNotEqual"] = "aNameNotEqual";
			parameters["nameLike"] = "aNameLike";
			parameters["nameNotLike"] = "aNameNotLike";
			parameters["owner"] = "anOwner";
			parameters["caseDefinitionKey"] = "aCaseDefKey";
			parameters["caseDefinitionId"] = "aCaseDefId";
			parameters["caseDefinitionName"] = "aCaseDefName";
			parameters["caseDefinitionNameLike"] = "aCaseDefNameLike";
			parameters["caseInstanceId"] = "anCaseInstanceId";
			parameters["caseInstanceBusinessKey"] = "aCaseInstanceBusinessKey";
			parameters["caseInstanceBusinessKeyLike"] = "aCaseInstanceBusinessKeyLike";
			parameters["caseExecutionId"] = "aCaseExecutionId";
			parameters["parentTaskId"] = "aParentTaskId";
    
			return parameters;
		  }
	  }

	  private IDictionary<string, bool> CompleteBooleanQueryParameters
	  {
		  get
		  {
			IDictionary<string, bool> parameters = new Dictionary<string, bool>();
    
			parameters["assigned"] = true;
			parameters["unassigned"] = true;
			parameters["active"] = true;
			parameters["suspended"] = true;
			parameters["withoutTenantId"] = true;
			parameters["withCandidateGroups"] = true;
			parameters["withoutCandidateGroups"] = true;
			parameters["withCandidateUsers"] = true;
			parameters["withoutCandidateUsers"] = true;
    
			return parameters;
		  }
	  }

	  private void verifyStringParameterQueryInvocations()
	  {
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;

		verify(mockQuery).processInstanceBusinessKey(stringQueryParameters["processInstanceBusinessKey"]);
		verify(mockQuery).processInstanceBusinessKeyLike(stringQueryParameters["processInstanceBusinessKeyLike"]);
		verify(mockQuery).processDefinitionKey(stringQueryParameters["processDefinitionKey"]);
		verify(mockQuery).processDefinitionId(stringQueryParameters["processDefinitionId"]);
		verify(mockQuery).executionId(stringQueryParameters["executionId"]);
		verify(mockQuery).processDefinitionName(stringQueryParameters["processDefinitionName"]);
		verify(mockQuery).processDefinitionNameLike(stringQueryParameters["processDefinitionNameLike"]);
		verify(mockQuery).processInstanceId(stringQueryParameters["processInstanceId"]);
		verify(mockQuery).taskAssignee(stringQueryParameters["assignee"]);
		verify(mockQuery).taskAssigneeLike(stringQueryParameters["assigneeLike"]);
		verify(mockQuery).taskCandidateGroup(stringQueryParameters["candidateGroup"]);
		verify(mockQuery).taskCandidateUser(stringQueryParameters["candidateUser"]);
		verify(mockQuery).taskDefinitionKey(stringQueryParameters["taskDefinitionKey"]);
		verify(mockQuery).taskDefinitionKeyLike(stringQueryParameters["taskDefinitionKeyLike"]);
		verify(mockQuery).taskDescription(stringQueryParameters["description"]);
		verify(mockQuery).taskDescriptionLike(stringQueryParameters["descriptionLike"]);
		verify(mockQuery).taskInvolvedUser(stringQueryParameters["involvedUser"]);
		verify(mockQuery).taskName(stringQueryParameters["name"]);
		verify(mockQuery).taskNameNotEqual(stringQueryParameters["nameNotEqual"]);
		verify(mockQuery).taskNameLike(stringQueryParameters["nameLike"]);
		verify(mockQuery).taskNameNotLike(stringQueryParameters["nameNotLike"]);
		verify(mockQuery).taskOwner(stringQueryParameters["owner"]);
		verify(mockQuery).caseDefinitionKey(stringQueryParameters["caseDefinitionKey"]);
		verify(mockQuery).caseDefinitionId(stringQueryParameters["caseDefinitionId"]);
		verify(mockQuery).caseDefinitionName(stringQueryParameters["caseDefinitionName"]);
		verify(mockQuery).caseDefinitionNameLike(stringQueryParameters["caseDefinitionNameLike"]);
		verify(mockQuery).caseInstanceId(stringQueryParameters["caseInstanceId"]);
		verify(mockQuery).caseInstanceBusinessKey(stringQueryParameters["caseInstanceBusinessKey"]);
		verify(mockQuery).caseInstanceBusinessKeyLike(stringQueryParameters["caseInstanceBusinessKeyLike"]);
		verify(mockQuery).caseExecutionId(stringQueryParameters["caseExecutionId"]);
		verify(mockQuery).taskParentTaskId(stringQueryParameters["parentTaskId"]);

	  }

	  private void verifyStringArrayParametersInvocations()
	  {
		IDictionary<string, String[]> stringArrayParameters = CompleteStringArrayQueryParameters;

		verify(mockQuery).activityInstanceIdIn(stringArrayParameters["activityInstanceIdIn"]);
		verify(mockQuery).taskDefinitionKeyIn(stringArrayParameters["taskDefinitionKeyIn"]);
		verify(mockQuery).processDefinitionKeyIn(stringArrayParameters["processDefinitionKeyIn"]);
		verify(mockQuery).processInstanceBusinessKeyIn(stringArrayParameters["processInstanceBusinessKeyIn"]);
		verify(mockQuery).tenantIdIn(stringArrayParameters["tenantIdIn"]);
	  }

	  private void verifyBooleanParameterQueryInvocation()
	  {
		verify(mockQuery).taskUnassigned();
		verify(mockQuery).active();
		verify(mockQuery).suspended();
		verify(mockQuery).withoutTenantId();
		verify(mockQuery).withCandidateGroups();
		verify(mockQuery).withoutCandidateGroups();
		verify(mockQuery).withCandidateUsers();
		verify(mockQuery).withoutCandidateUsers();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDateParameters()
	  public virtual void testDateParameters()
	  {
		IDictionary<string, string> queryParameters = DateParameters;

		given().queryParams(queryParameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).dueAfter(any(typeof(DateTime)));
		verify(mockQuery).dueBefore(any(typeof(DateTime)));
		verify(mockQuery).dueDate(any(typeof(DateTime)));
		verify(mockQuery).followUpAfter(any(typeof(DateTime)));
		verify(mockQuery).followUpBefore(any(typeof(DateTime)));
		verify(mockQuery).followUpBeforeOrNotExistent(any(typeof(DateTime)));
		verify(mockQuery).followUpDate(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedAfter(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedBefore(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedOn(any(typeof(DateTime)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDateParametersPost()
	  public virtual void testDateParametersPost()
	  {
		IDictionary<string, string> json = DateParameters;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).dueAfter(any(typeof(DateTime)));
		verify(mockQuery).dueBefore(any(typeof(DateTime)));
		verify(mockQuery).dueDate(any(typeof(DateTime)));
		verify(mockQuery).followUpAfter(any(typeof(DateTime)));
		verify(mockQuery).followUpBefore(any(typeof(DateTime)));
		verify(mockQuery).followUpBeforeOrNotExistent(any(typeof(DateTime)));
		verify(mockQuery).followUpDate(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedAfter(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedBefore(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedOn(any(typeof(DateTime)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeprecatedDateParameters()
	  public virtual void testDeprecatedDateParameters()
	  {
		IDictionary<string, string> queryParameters = new Dictionary<string, string>();
		queryParameters["due"] = withTimezone("2013-01-23T14:42:44");
		queryParameters["created"] = withTimezone("2013-01-23T14:42:47");
		queryParameters["followUp"] = withTimezone("2013-01-23T14:42:50");

		given().queryParams(queryParameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).dueDate(any(typeof(DateTime)));
		verify(mockQuery).taskCreatedOn(any(typeof(DateTime)));
		verify(mockQuery).followUpDate(any(typeof(DateTime)));
	  }

	  private IDictionary<string, string> DateParameters
	  {
		  get
		  {
			IDictionary<string, string> parameters = new Dictionary<string, string>();
			parameters["dueAfter"] = withTimezone("2013-01-23T14:42:42");
			parameters["dueBefore"] = withTimezone("2013-01-23T14:42:43");
			parameters["dueDate"] = withTimezone("2013-01-23T14:42:44");
			parameters["createdAfter"] = withTimezone("2013-01-23T14:42:45");
			parameters["createdBefore"] = withTimezone("2013-01-23T14:42:46");
			parameters["createdOn"] = withTimezone("2013-01-23T14:42:47");
			parameters["followUpAfter"] = withTimezone("2013-01-23T14:42:48");
			parameters["followUpBefore"] = withTimezone("2013-01-23T14:42:49");
			parameters["followUpBeforeOrNotExistent"] = withTimezone("2013-01-23T14:42:49");
			parameters["followUpDate"] = withTimezone("2013-01-23T14:42:50");
			return parameters;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCandidateGroupInList()
	  public virtual void testCandidateGroupInList()
	  {
		IList<string> candidateGroups = new List<string>();
		candidateGroups.Add("boss");
		candidateGroups.Add("worker");
		string queryParam = candidateGroups[0] + "," + candidateGroups[1];

		given().queryParams("candidateGroups", queryParam).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskCandidateGroupIn(argThat(new EqualsList(candidateGroups)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDelegationState()
	  public virtual void testDelegationState()
	  {
		given().queryParams("delegationState", "PENDING").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskDelegationState(DelegationState.PENDING);

		given().queryParams("delegationState", "RESOLVED").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskDelegationState(DelegationState.RESOLVED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLowerCaseDelegationStateParam()
	  public virtual void testLowerCaseDelegationStateParam()
	  {
		given().queryParams("delegationState", "resolved").header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskDelegationState(DelegationState.RESOLVED);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortingParameters()
	  public virtual void testSortingParameters()
	  {
		InOrder inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("dueDate", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByDueDate();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("followUpDate", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByFollowUpDate();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("instanceId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessInstanceId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("created", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskCreateTime();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("id", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("priority", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskPriority();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("executionId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByExecutionId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("assignee", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskAssignee();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("description", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskDescription();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("name", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskName();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("nameCaseInsensitive", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskNameCaseInsensitive();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("caseInstanceId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByCaseInstanceId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("dueDate", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByDueDate();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("followUpDate", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByFollowUpDate();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("instanceId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByProcessInstanceId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("created", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskCreateTime();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("id", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("priority", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskPriority();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("executionId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByExecutionId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("assignee", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskAssignee();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("description", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskDescription();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("name", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskName();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("nameCaseInsensitive", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTaskNameCaseInsensitive();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("caseInstanceId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByCaseInstanceId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("tenantId", "desc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).desc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySorting("tenantId", "asc", Status.OK);
		inOrder.verify(mockQuery).orderByTenantId();
		inOrder.verify(mockQuery).asc();

	  }

	  protected internal virtual void executeAndVerifySorting(string sortBy, string sortOrder, Status expectedStatus)
	  {
		given().queryParam("sortBy", sortBy).queryParam("sortOrder", sortOrder).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(expectedStatus.StatusCode).when().get(TASK_QUERY_URL);
	  }

	  protected internal virtual void executeAndVerifySortingAsPost(IList<IDictionary<string, object>> sortingJson, Status expectedStatus)
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();
		json["sorting"] = sortingJson;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSecondarySortingAsPost()
	  public virtual void testSecondarySortingAsPost()
	  {
		InOrder inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySortingAsPost(OrderingBuilder.create().orderBy("dueDate").desc().orderBy("caseExecutionId").asc().Json, Status.OK);

		inOrder.verify(mockQuery).orderByDueDate();
		inOrder.verify(mockQuery).desc();
		inOrder.verify(mockQuery).orderByCaseExecutionId();
		inOrder.verify(mockQuery).asc();

		inOrder = Mockito.inOrder(mockQuery);
		executeAndVerifySortingAsPost(OrderingBuilder.create().orderBy("processVariable").desc().parameter("variable", "var").parameter("type", "String").orderBy("executionVariable").asc().parameter("variable", "var2").parameter("type", "Integer").orderBy("taskVariable").desc().parameter("variable", "var3").parameter("type", "Double").orderBy("caseInstanceVariable").asc().parameter("variable", "var4").parameter("type", "Long").orderBy("caseExecutionVariable").desc().parameter("variable", "var5").parameter("type", "Date").Json, Status.OK);

		inOrder.verify(mockQuery).orderByProcessVariable("var", ValueType.STRING);
		inOrder.verify(mockQuery).desc();
		inOrder.verify(mockQuery).orderByExecutionVariable("var2", ValueType.INTEGER);
		inOrder.verify(mockQuery).asc();
		inOrder.verify(mockQuery).orderByTaskVariable("var3", ValueType.DOUBLE);
		inOrder.verify(mockQuery).desc();
		inOrder.verify(mockQuery).orderByCaseInstanceVariable("var4", ValueType.LONG);
		inOrder.verify(mockQuery).asc();
		inOrder.verify(mockQuery).orderByCaseExecutionVariable("var5", ValueType.DATE);
		inOrder.verify(mockQuery).desc();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSuccessfulPagination()
	  public virtual void testSuccessfulPagination()
	  {

		int firstResult = 0;
		int maxResults = 10;
		given().queryParam("firstResult", firstResult).queryParam("maxResults", maxResults).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).listPage(firstResult, maxResults);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableParameters()
	  public virtual void testTaskVariableParameters()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("taskVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("taskVariables", queryValue).queryParam("variableNamesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("taskVariables", queryValue).queryParam("variableNamesIgnoreCase", true).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(variableName, variableValue);
		// greater than
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueGreaterThan(variableName, variableValue);

		// greater than equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueGreaterThanOrEquals(variableName, variableValue);

		// lower than
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueLessThan(variableName, variableValue);

		// lower than equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueLessThanOrEquals(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueLike(variableName, variableValue);
		reset(mockQuery);

		// like case-insensitive
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("taskVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueNotEquals(variableName, variableValue);
		reset(mockQuery);

		// not equals case-insensitive
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("taskVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableValueEqualsIgnoreCaseAsPost()
	  public virtual void testTaskVariableValueEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableNameEqualsIgnoreCaseAsPost()
	  public virtual void testTaskVariableNameEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME.ToLower();
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskVariables"] = variables;
		json["variableNamesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);
		reset(mockQuery);

		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableValueNotEqualsIgnoreCaseAsPost()
	  public virtual void testTaskVariableValueNotEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "neq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueNotEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTaskVariableValueLikeIgnoreCaseAsPost()
	  public virtual void testTaskVariableValueLikeIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "like";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).taskVariableValueLike(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableParameters()
	  public virtual void testProcessVariableParameters()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		//equals case-insensitive
		queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("processVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("processVariables", queryValue).queryParam("variableNamesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("processVariables", queryValue).queryParam("variableNamesIgnoreCase", true).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(variableName, variableValue);

		// greater than
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueGreaterThan(variableName, variableValue);

		// greater than equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueGreaterThanOrEquals(variableName, variableValue);

		// lower than
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueLessThan(variableName, variableValue);

		// lower than equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueLessThanOrEquals(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueLike(variableName, variableValue);
		reset(mockQuery);

		// like case-insensitive
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("processVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueNotEquals(variableName, variableValue);
		reset(mockQuery);

		// not equals case-insensitive
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("processVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableValueEqualsIgnoreCaseAsPost()
	  public virtual void testProcessVariableValueEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableNameEqualsIgnoreCaseAsPost()
	  public virtual void testProcessVariableNameEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME.ToLower();
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processVariables"] = variables;
		json["variableNamesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);
		reset(mockQuery);

		json["variableValuesIgnoreCase"] = true;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableValueNotEqualsIgnoreCaseAsPost()
	  public virtual void testProcessVariableValueNotEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "neq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueNotEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProcessVariableValueLikeIgnoreCaseAsPost()
	  public virtual void testProcessVariableValueLikeIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "like";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).processVariableValueLike(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseVariableParameters()
	  public virtual void testCaseVariableParameters()
	  {
		// equals
		string variableName = "varName";
		string variableValue = "varValue";
		string queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		// equals case-insensitive
		queryValue = variableName + "_eq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("caseInstanceVariables", queryValue).queryParam("variableNamesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(variableName, variableValue);
		reset(mockQuery);

		given().queryParam("caseInstanceVariables", queryValue).queryParam("variableNamesIgnoreCase", true).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(variableName, variableValue);

		// greater than
		queryValue = variableName + "_gt_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueGreaterThan(variableName, variableValue);

		// greater than equals
		queryValue = variableName + "_gteq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueGreaterThanOrEquals(variableName, variableValue);

		// lower than
		queryValue = variableName + "_lt_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueLessThan(variableName, variableValue);

		// lower than equals
		queryValue = variableName + "_lteq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueLessThanOrEquals(variableName, variableValue);

		// like
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueLike(variableName, variableValue);
		reset(mockQuery);

		// like case-insensitive
		queryValue = variableName + "_like_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueLike(variableName, variableValue);

		// not equals
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueNotEquals(variableName, variableValue);
		reset(mockQuery);

		// not equals case-insensitive
		queryValue = variableName + "_neq_" + variableValue;

		given().queryParam("caseInstanceVariables", queryValue).queryParam("variableValuesIgnoreCase", true).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueNotEquals(variableName, variableValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceVariableValueEqualsIgnoreCaseAsPost()
	  public virtual void testCaseInstanceVariableValueEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceVariableNameEqualsIgnoreCaseAsPost()
	  public virtual void testCaseInstanceVariableNameEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME.ToLower();
		variableJson["operator"] = "eq";
		variableJson["value"] = SAMPLE_VAR_VALUE;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceVariables"] = variables;
		json["variableNamesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);
		reset(mockQuery);

		json["variableValuesIgnoreCase"] = true;
		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableNamesIgnoreCase();
		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueEquals(SAMPLE_VAR_NAME.ToLower(), SAMPLE_VAR_VALUE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceVariableValueNotEqualsIgnoreCaseAsPost()
	  public virtual void testCaseInstanceVariableValueNotEqualsIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "neq";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueNotEquals(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCaseInstanceVariableValueLikeIgnoreCaseAsPost()
	  public virtual void testCaseInstanceVariableValueLikeIgnoreCaseAsPost()
	  {
		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = SAMPLE_VAR_NAME;
		variableJson["operator"] = "like";
		variableJson["value"] = SAMPLE_VAR_VALUE.ToLower();

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceVariables"] = variables;
		json["variableValuesIgnoreCase"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).matchVariableValuesIgnoreCase();
		verify(mockQuery).caseInstanceVariableValueLike(SAMPLE_VAR_NAME, SAMPLE_VAR_VALUE.ToLower());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleVariableParameters()
	  public virtual void testMultipleVariableParameters()
	  {
		string variableName1 = "varName";
		string variableValue1 = "varValue";
		string variableParameter1 = variableName1 + "_eq_" + variableValue1;

		string variableName2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";
		string variableParameter2 = variableName2 + "_neq_" + variableValue2;

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam("taskVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueEquals(variableName1, variableValue1);
		verify(mockQuery).taskVariableValueNotEquals(variableName2, variableValue2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleVariableParametersAsPost()
	  public virtual void testMultipleVariableParametersAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string anotherVariableName = "anotherVarName";
		int? anotherVariableValue = 30;

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IDictionary<string, object> anotherVariableJson = new Dictionary<string, object>();
		anotherVariableJson["name"] = anotherVariableName;
		anotherVariableJson["operator"] = "neq";
		anotherVariableJson["value"] = anotherVariableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);
		variables.Add(anotherVariableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["taskVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).taskVariableValueEquals(variableName, variableValue);
		verify(mockQuery).taskVariableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleProcessVariableParameters()
	  public virtual void testMultipleProcessVariableParameters()
	  {
		string variableName1 = "varName";
		string variableValue1 = "varValue";
		string variableParameter1 = variableName1 + "_eq_" + variableValue1;

		string variableName2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";
		string variableParameter2 = variableName2 + "_neq_" + variableValue2;

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam("processVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueEquals(variableName1, variableValue1);
		verify(mockQuery).processVariableValueNotEquals(variableName2, variableValue2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleProcessVariableParametersAsPost()
	  public virtual void testMultipleProcessVariableParametersAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string anotherVariableName = "anotherVarName";
		int? anotherVariableValue = 30;

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IDictionary<string, object> anotherVariableJson = new Dictionary<string, object>();
		anotherVariableJson["name"] = anotherVariableName;
		anotherVariableJson["operator"] = "neq";
		anotherVariableJson["value"] = anotherVariableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);
		variables.Add(anotherVariableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["processVariables"] = variables;

		given().header("accept", MediaType.APPLICATION_JSON).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).processVariableValueEquals(variableName, variableValue);
		verify(mockQuery).processVariableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleCaseVariableParameters()
	  public virtual void testMultipleCaseVariableParameters()
	  {
		string variableName1 = "varName";
		string variableValue1 = "varValue";
		string variableParameter1 = variableName1 + "_eq_" + variableValue1;

		string variableName2 = "anotherVarName";
		string variableValue2 = "anotherVarValue";
		string variableParameter2 = variableName2 + "_neq_" + variableValue2;

		string queryValue = variableParameter1 + "," + variableParameter2;

		given().queryParam("caseInstanceVariables", queryValue).header("accept", MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueEquals(variableName1, variableValue1);
		verify(mockQuery).caseInstanceVariableValueNotEquals(variableName2, variableValue2);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleCaseVariableParametersAsPost()
	  public virtual void testMultipleCaseVariableParametersAsPost()
	  {
		string variableName = "varName";
		string variableValue = "varValue";
		string anotherVariableName = "anotherVarName";
		int? anotherVariableValue = 30;

		IDictionary<string, object> variableJson = new Dictionary<string, object>();
		variableJson["name"] = variableName;
		variableJson["operator"] = "eq";
		variableJson["value"] = variableValue;

		IDictionary<string, object> anotherVariableJson = new Dictionary<string, object>();
		anotherVariableJson["name"] = anotherVariableName;
		anotherVariableJson["operator"] = "neq";
		anotherVariableJson["value"] = anotherVariableValue;

		IList<IDictionary<string, object>> variables = new List<IDictionary<string, object>>();
		variables.Add(variableJson);
		variables.Add(anotherVariableJson);

		IDictionary<string, object> json = new Dictionary<string, object>();
		json["caseInstanceVariables"] = variables;

		given().contentType(POST_JSON_CONTENT_TYPE).header("accept", MediaType.APPLICATION_JSON).body(json).then().expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verify(mockQuery).caseInstanceVariableValueEquals(variableName, variableValue);
		verify(mockQuery).caseInstanceVariableValueNotEquals(eq(anotherVariableName), argThat(EqualsPrimitiveValue.numberValue(anotherVariableValue)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCompletePostParameters()
	  public virtual void testCompletePostParameters()
	  {

		IDictionary<string, object> queryParameters = new Dictionary<string, object>();
		IDictionary<string, string> stringQueryParameters = CompleteStringQueryParameters;
		IDictionary<string, int> intQueryParameters = CompleteIntQueryParameters;
		IDictionary<string, bool> booleanQueryParameters = CompleteBooleanQueryParameters;
		IDictionary<string, String[]> stringArrayQueryParameters = CompleteStringArrayQueryParameters;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		queryParameters.putAll(stringQueryParameters);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		queryParameters.putAll(intQueryParameters);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		queryParameters.putAll(booleanQueryParameters);
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		queryParameters.putAll(stringArrayQueryParameters);

		IList<string> candidateGroups = new List<string>();
		candidateGroups.Add("boss");
		candidateGroups.Add("worker");

		queryParameters["candidateGroups"] = candidateGroups;

		queryParameters["includeAssignedTasks"] = true;

		given().contentType(POST_JSON_CONTENT_TYPE).body(queryParameters).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verifyStringParameterQueryInvocations();
		verifyIntegerParameterQueryInvocations();
		verifyStringArrayParametersInvocations();
		verifyBooleanParameterQueryInvocation();

		verify(mockQuery).includeAssignedTasks();
		verify(mockQuery).taskCandidateGroupIn(argThat(new EqualsList(candidateGroups)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCount()
	  public virtual void testQueryCount()
	  {
		given().header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(TASK_COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryCountForPost()
	  public virtual void testQueryCountForPost()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).header("accept", MediaType.APPLICATION_JSON).expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(TASK_COUNT_QUERY_URL);

		verify(mockQuery).count();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithExpressions()
	  public virtual void testQueryWithExpressions()
	  {
		string testExpression = "${'test-%s'}";

		ValueGenerator generator = new ValueGenerator(testExpression);

		IDictionary<string, string> @params = new Dictionary<string, string>();
		@params["assigneeExpression"] = generator.getValue("assigneeExpression");
		@params["assigneeLikeExpression"] = generator.getValue("assigneeLikeExpression");
		@params["ownerExpression"] = generator.getValue("ownerExpression");
		@params["involvedUserExpression"] = generator.getValue("involvedUserExpression");
		@params["candidateUserExpression"] = generator.getValue("candidateUserExpression");
		@params["candidateGroupExpression"] = generator.getValue("candidateGroupExpression");
		@params["candidateGroupsExpression"] = generator.getValue("candidateGroupsExpression");
		@params["createdBeforeExpression"] = generator.getValue("createdBeforeExpression");
		@params["createdOnExpression"] = generator.getValue("createdOnExpression");
		@params["createdAfterExpression"] = generator.getValue("createdAfterExpression");
		@params["dueBeforeExpression"] = generator.getValue("dueBeforeExpression");
		@params["dueDateExpression"] = generator.getValue("dueDateExpression");
		@params["dueAfterExpression"] = generator.getValue("dueAfterExpression");
		@params["followUpBeforeExpression"] = generator.getValue("followUpBeforeExpression");
		@params["followUpDateExpression"] = generator.getValue("followUpDateExpression");
		@params["followUpAfterExpression"] = generator.getValue("followUpAfterExpression");
		@params["processInstanceBusinessKeyExpression"] = generator.getValue("processInstanceBusinessKeyExpression");
		@params["processInstanceBusinessKeyLikeExpression"] = generator.getValue("processInstanceBusinessKeyLikeExpression");

		// get
		given().header(ACCEPT_JSON_HEADER).queryParams(@params).expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verifyExpressionMocks(generator);

		// reset mock
		reset(mockQuery);

		// post
		given().contentType(POST_JSON_CONTENT_TYPE).header(ACCEPT_JSON_HEADER).body(@params).expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		verifyExpressionMocks(generator);

	  }

	  protected internal virtual void verifyExpressionMocks(ValueGenerator generator)
	  {
		verify(mockQuery).taskAssigneeExpression(generator.getValue("assigneeExpression"));
		verify(mockQuery).taskAssigneeLikeExpression(generator.getValue("assigneeLikeExpression"));
		verify(mockQuery).taskOwnerExpression(generator.getValue("ownerExpression"));
		verify(mockQuery).taskInvolvedUserExpression(generator.getValue("involvedUserExpression"));
		verify(mockQuery).taskCandidateUserExpression(generator.getValue("candidateUserExpression"));
		verify(mockQuery).taskCandidateGroupExpression(generator.getValue("candidateGroupExpression"));
		verify(mockQuery).taskCandidateGroupInExpression(generator.getValue("candidateGroupsExpression"));
		verify(mockQuery).taskCreatedBeforeExpression(generator.getValue("createdBeforeExpression"));
		verify(mockQuery).taskCreatedOnExpression(generator.getValue("createdOnExpression"));
		verify(mockQuery).taskCreatedAfterExpression(generator.getValue("createdAfterExpression"));
		verify(mockQuery).dueBeforeExpression(generator.getValue("dueBeforeExpression"));
		verify(mockQuery).dueDateExpression(generator.getValue("dueDateExpression"));
		verify(mockQuery).dueAfterExpression(generator.getValue("dueAfterExpression"));
		verify(mockQuery).followUpBeforeExpression(generator.getValue("followUpBeforeExpression"));
		verify(mockQuery).followUpDateExpression(generator.getValue("followUpDateExpression"));
		verify(mockQuery).followUpAfterExpression(generator.getValue("followUpAfterExpression"));
		verify(mockQuery).processInstanceBusinessKeyExpression(generator.getValue("processInstanceBusinessKeyExpression"));
		verify(mockQuery).processInstanceBusinessKeyLikeExpression(generator.getValue("processInstanceBusinessKeyLikeExpression"));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithCandidateUsers()
	  public virtual void testQueryWithCandidateUsers()
	  {
		given().queryParam("withCandidateUsers", true).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).withCandidateUsers();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQueryWithoutCandidateUsers()
	  public virtual void testQueryWithoutCandidateUsers()
	  {
		given().queryParam("withoutCandidateUsers", true).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery).withoutCandidateUsers();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNeverQueryWithCandidateUsers()
	  public virtual void testNeverQueryWithCandidateUsers()
	  {
		given().queryParam("withCandidateUsers", false).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery, never()).withCandidateUsers();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNeverQueryWithoutCandidateUsers()
	  public virtual void testNeverQueryWithoutCandidateUsers()
	  {
		given().queryParam("withoutCandidateUsers", false).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery, never()).withoutCandidateUsers();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNeverQueryWithCandidateGroups()
	  public virtual void testNeverQueryWithCandidateGroups()
	  {
		given().queryParam("withCandidateGroups", false).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery, never()).withCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNeverQueryWithoutCandidateGroups()
	  public virtual void testNeverQueryWithoutCandidateGroups()
	  {
		given().queryParam("withoutCandidateGroups", false).accept(MediaType.APPLICATION_JSON).then().expect().statusCode(Status.OK.StatusCode).when().get(TASK_QUERY_URL);

		verify(mockQuery, never()).withoutCandidateGroups();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrQuery()
	  public virtual void testOrQuery()
	  {
		TaskQueryDto queryDto = TaskQueryDto.fromQuery(new TaskQueryImpl()
		  .or().taskName(MockProvider.EXAMPLE_TASK_NAME).taskDescription(MockProvider.EXAMPLE_TASK_DESCRIPTION).endOr());

		given().contentType(POST_JSON_CONTENT_TYPE).header(ACCEPT_JSON_HEADER).body(queryDto).then().expect().statusCode(Status.OK.StatusCode).when().post(TASK_QUERY_URL);

		ArgumentCaptor<TaskQueryImpl> argument = ArgumentCaptor.forClass(typeof(TaskQueryImpl));
		verify(((TaskQueryImpl) mockQuery)).addOrQuery(argument.capture());
		assertEquals(MockProvider.EXAMPLE_TASK_NAME, argument.Value.Name);
		assertEquals(MockProvider.EXAMPLE_TASK_DESCRIPTION, argument.Value.Description);
	  }

	}

}