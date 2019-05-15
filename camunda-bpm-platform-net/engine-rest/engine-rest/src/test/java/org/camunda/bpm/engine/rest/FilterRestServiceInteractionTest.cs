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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.FILTER;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.AbstractQueryDto.SORT_ORDER_ASC_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.AbstractQueryDto.SORT_ORDER_DESC_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_ASSIGNEE_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_CASE_EXECUTION_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_CASE_EXECUTION_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_CASE_INSTANCE_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_CASE_INSTANCE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_CREATE_TIME_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_DESCRIPTION_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_DUE_DATE_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_EXECUTION_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_EXECUTION_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_FOLLOW_UP_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_NAME_CASE_INSENSITIVE_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_NAME_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_PRIORITY_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_PROCESS_INSTANCE_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_PROCESS_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_TASK_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.dto.task.TaskQueryDto.SORT_BY_TENANT_ID_VALUE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.EXAMPLE_FILTER_ID;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.mockFilter;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.MockProvider.mockVariableInstance;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.TaskQueryMatcher.hasName;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.variable.Variables.stringValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.hasItems;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.isEmptyOrNullString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyVararg;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.argThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.eq;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.isNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.doThrow;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.never;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;



	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using FilterQuery = org.camunda.bpm.engine.filter.FilterQuery;
	using AuthorizationServiceImpl = org.camunda.bpm.engine.impl.AuthorizationServiceImpl;
	using IdentityServiceImpl = org.camunda.bpm.engine.impl.IdentityServiceImpl;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using FilterEntity = org.camunda.bpm.engine.impl.persistence.entity.FilterEntity;
	using TaskEntity = org.camunda.bpm.engine.impl.persistence.entity.TaskEntity;
	using Query = org.camunda.bpm.engine.query.Query;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using TaskQueryDto = org.camunda.bpm.engine.rest.dto.task.TaskQueryDto;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using MockTaskBuilder = org.camunda.bpm.engine.rest.helper.MockTaskBuilder;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Test = org.junit.Test;

	using Response = io.restassured.response.Response;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class FilterRestServiceInteractionTest : AbstractRestServiceTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
	  public static TestContainerRule rule = new TestContainerRule();

	  public static readonly string FILTER_URL = TEST_RESOURCE_ROOT_PATH + FilterRestService_Fields.PATH;
	  public static readonly string SINGLE_FILTER_URL = FILTER_URL + "/{id}";
	  public static readonly string CREATE_FILTER_URL = FILTER_URL + "/create";
	  public static readonly string EXECUTE_SINGLE_RESULT_FILTER_URL = SINGLE_FILTER_URL + "/singleResult";
	  public static readonly string EXECUTE_LIST_FILTER_URL = SINGLE_FILTER_URL + "/list";
	  public static readonly string EXECUTE_COUNT_FILTER_URL = SINGLE_FILTER_URL + "/count";

	  public static readonly TaskQuery extendingQuery = new TaskQueryImpl().taskName(MockProvider.EXAMPLE_TASK_NAME);
	  public static readonly TaskQueryDto extendingQueryDto = TaskQueryDto.fromQuery(extendingQuery);
	  public static readonly TaskQuery extendingOrQuery = new TaskQueryImpl().or().taskDescription(MockProvider.EXAMPLE_TASK_DESCRIPTION).endOr().or().taskName(MockProvider.EXAMPLE_TASK_NAME).endOr();
	  public static readonly TaskQueryDto extendingOrQueryDto = TaskQueryDto.fromQuery(extendingOrQuery);
	  public const string invalidExtendingQuery = "abc";

	  public const string PROCESS_INSTANCE_A_ID = "processInstanceA";
	  public const string CASE_INSTANCE_A_ID = "caseInstanceA";
	  public const string EXECUTION_A_ID = "executionA";
	  public const string EXECUTION_B_ID = "executionB";
	  public const string CASE_EXECUTION_A_ID = "caseExecutionA";
	  public const string TASK_A_ID = "taskA";
	  public const string TASK_B_ID = "taskB";
	  public const string TASK_C_ID = "taskC";

	  protected internal FilterService filterServiceMock;
	  protected internal Filter filterMock;

	  protected internal AuthorizationService authorizationServiceMock;
	  protected internal IdentityService identityServiceMock;
	  protected internal VariableInstanceQuery variableInstanceQueryMock;
	  protected internal ProcessEngineConfiguration processEngineConfigurationMock;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before @SuppressWarnings("unchecked") public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		filterServiceMock = mock(typeof(FilterService));

		when(processEngine.FilterService).thenReturn(filterServiceMock);

		FilterQuery filterQuery = MockProvider.createMockFilterQuery();

		when(filterServiceMock.createFilterQuery()).thenReturn(filterQuery);

		filterMock = MockProvider.createMockFilter();

		when(filterServiceMock.newTaskFilter()).thenReturn(filterMock);
		when(filterServiceMock.saveFilter(eq(filterMock))).thenReturn(filterMock);
		when(filterServiceMock.getFilter(eq(EXAMPLE_FILTER_ID))).thenReturn(filterMock);
		when(filterServiceMock.getFilter(eq(MockProvider.NON_EXISTING_ID))).thenReturn(null);

		IList<object> mockTasks = Collections.singletonList<object>(new TaskEntity());

		when(filterServiceMock.singleResult(eq(EXAMPLE_FILTER_ID))).thenReturn(mockTasks[0]);
		when(filterServiceMock.singleResult(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(mockTasks[0]);
		when(filterServiceMock.list(eq(EXAMPLE_FILTER_ID))).thenReturn(mockTasks);
		when(filterServiceMock.list(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(mockTasks);
		when(filterServiceMock.listPage(eq(EXAMPLE_FILTER_ID), anyInt(), anyInt())).thenReturn(mockTasks);
		when(filterServiceMock.listPage(eq(EXAMPLE_FILTER_ID), any(typeof(Query)), anyInt(), anyInt())).thenReturn(mockTasks);
		when(filterServiceMock.count(eq(EXAMPLE_FILTER_ID))).thenReturn((long) 1);
		when(filterServiceMock.count(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn((long) 1);

		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).singleResult(eq(MockProvider.NON_EXISTING_ID));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).singleResult(eq(MockProvider.NON_EXISTING_ID), any(typeof(Query)));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).list(eq(MockProvider.NON_EXISTING_ID));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).list(eq(MockProvider.NON_EXISTING_ID), any(typeof(Query)));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).listPage(eq(MockProvider.NON_EXISTING_ID), anyInt(), anyInt());
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).listPage(eq(MockProvider.NON_EXISTING_ID), any(typeof(Query)), anyInt(), anyInt());
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).count(eq(MockProvider.NON_EXISTING_ID));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).count(eq(MockProvider.NON_EXISTING_ID), any(typeof(Query)));
		doThrow(new NullValueException("No filter found with given id")).when(filterServiceMock).deleteFilter(eq(MockProvider.NON_EXISTING_ID));

		authorizationServiceMock = mock(typeof(AuthorizationServiceImpl));
		identityServiceMock = mock(typeof(IdentityServiceImpl));
		processEngineConfigurationMock = mock(typeof(ProcessEngineConfiguration));

		when(processEngine.AuthorizationService).thenReturn(authorizationServiceMock);
		when(processEngine.IdentityService).thenReturn(identityServiceMock);
		when(processEngine.ProcessEngineConfiguration).thenReturn(processEngineConfigurationMock);

		TaskService taskService = processEngine.TaskService;
		when(taskService.createTaskQuery()).thenReturn(new TaskQueryImpl());

		variableInstanceQueryMock = mock(typeof(VariableInstanceQuery));
		when(processEngine.RuntimeService.createVariableInstanceQuery()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.variableScopeIdIn((string) anyVararg())).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.variableNameIn((string) anyVararg())).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableBinaryFetching()).thenReturn(variableInstanceQueryMock);
		when(variableInstanceQueryMock.disableCustomObjectDeserialization()).thenReturn(variableInstanceQueryMock);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilter()
	  public virtual void testGetFilter()
	  {
		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("id", equalTo(EXAMPLE_FILTER_ID)).when().get(SINGLE_FILTER_URL);

		verify(filterServiceMock).getFilter(eq(EXAMPLE_FILTER_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetNonExistingFilter()
	  public virtual void testGetNonExistingFilter()
	  {
		given().pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(SINGLE_FILTER_URL);

		verify(filterServiceMock).getFilter(eq(MockProvider.NON_EXISTING_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithTaskQuery()
	  public virtual void testGetFilterWithTaskQuery()
	  {
		TaskQueryImpl query = mock(typeof(TaskQueryImpl));
		when(query.Assignee).thenReturn(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME);
		when(query.AssigneeLike).thenReturn(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME);
		when(query.CaseDefinitionId).thenReturn(MockProvider.EXAMPLE_CASE_DEFINITION_ID);
		when(query.CaseDefinitionKey).thenReturn(MockProvider.EXAMPLE_CASE_DEFINITION_KEY);
		when(query.CaseDefinitionName).thenReturn(MockProvider.EXAMPLE_CASE_DEFINITION_NAME);
		when(query.CaseDefinitionNameLike).thenReturn(MockProvider.EXAMPLE_CASE_DEFINITION_NAME_LIKE);
		when(query.CaseExecutionId).thenReturn(MockProvider.EXAMPLE_CASE_EXECUTION_ID);
		when(query.CaseInstanceBusinessKey).thenReturn(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY);
		when(query.CaseInstanceBusinessKeyLike).thenReturn(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY_LIKE);
		when(query.CaseInstanceId).thenReturn(MockProvider.EXAMPLE_CASE_INSTANCE_ID);
		when(query.CandidateUser).thenReturn(MockProvider.EXAMPLE_USER_ID);
		when(query.CandidateGroup).thenReturn(MockProvider.EXAMPLE_GROUP_ID);
		when(query.ProcessInstanceBusinessKey).thenReturn(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY);
		when(query.ProcessInstanceBusinessKeyLike).thenReturn(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY_LIKE);
		when(query.ProcessDefinitionKey).thenReturn(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY);
		when(query.ProcessDefinitionKeys).thenReturn(new string[]{MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY});
		when(query.ProcessDefinitionId).thenReturn(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID);
		when(query.ExecutionId).thenReturn(MockProvider.EXAMPLE_EXECUTION_ID);
		when(query.ProcessDefinitionName).thenReturn(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME);
		when(query.ProcessDefinitionNameLike).thenReturn(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME_LIKE);
		when(query.ProcessInstanceId).thenReturn(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID);
		when(query.Key).thenReturn(MockProvider.EXAMPLE_TASK_DEFINITION_KEY);
		when(query.Keys).thenReturn(new string[]{MockProvider.EXAMPLE_TASK_DEFINITION_KEY, MockProvider.EXAMPLE_TASK_DEFINITION_KEY});
		when(query.KeyLike).thenReturn(MockProvider.EXAMPLE_TASK_DEFINITION_KEY);
		when(query.Description).thenReturn(MockProvider.EXAMPLE_TASK_DESCRIPTION);
		when(query.DescriptionLike).thenReturn(MockProvider.EXAMPLE_TASK_DESCRIPTION);
		when(query.InvolvedUser).thenReturn(MockProvider.EXAMPLE_USER_ID);
		when(query.Priority).thenReturn(1);
		when(query.MaxPriority).thenReturn(2);
		when(query.MinPriority).thenReturn(3);
		when(query.Name).thenReturn(MockProvider.EXAMPLE_TASK_NAME);
		when(query.NameLike).thenReturn(MockProvider.EXAMPLE_TASK_NAME);
		when(query.Owner).thenReturn(MockProvider.EXAMPLE_TASK_OWNER);
		when(query.ParentTaskId).thenReturn(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID);
		when(query.TenantIds).thenReturn(MockProvider.EXAMPLE_TENANT_ID_LIST.Split(",", true));
		when(query.TenantIdSet).thenReturn(true);

		filterMock = MockProvider.createMockFilter(EXAMPLE_FILTER_ID, query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filterMock);

		given().pathParam("id", MockProvider.EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.assignee", equalTo(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME)).body("query.assigneeLike", equalTo(MockProvider.EXAMPLE_TASK_ASSIGNEE_NAME)).body("query.caseDefinitionId", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_ID)).body("query.caseDefinitionKey", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_KEY)).body("query.caseDefinitionName", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_NAME)).body("query.caseDefinitionNameLike", equalTo(MockProvider.EXAMPLE_CASE_DEFINITION_NAME_LIKE)).body("query.caseExecutionId", equalTo(MockProvider.EXAMPLE_CASE_EXECUTION_ID)).body("query.caseInstanceBusinessKey", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY)).body("query.caseInstanceBusinessKeyLike", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_BUSINESS_KEY_LIKE)).body("query.caseInstanceId", equalTo(MockProvider.EXAMPLE_CASE_INSTANCE_ID)).body("query.candidateUser", equalTo(MockProvider.EXAMPLE_USER_ID)).body("query.candidateGroup", equalTo(MockProvider.EXAMPLE_GROUP_ID)).body("query.processInstanceBusinessKey", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY)).body("query.processInstanceBusinessKeyLike", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_BUSINESS_KEY_LIKE)).body("query.processDefinitionKey", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("query.processDefinitionKeyIn", hasItems(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY, MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("query.processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("query.executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("query.processDefinitionName", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME)).body("query.processDefinitionNameLike", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_NAME_LIKE)).body("query.processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("query.taskDefinitionKey", equalTo(MockProvider.EXAMPLE_TASK_DEFINITION_KEY)).body("query.taskDefinitionKeyIn", hasItems(MockProvider.EXAMPLE_TASK_DEFINITION_KEY, MockProvider.EXAMPLE_TASK_DEFINITION_KEY)).body("query.taskDefinitionKeyLike", equalTo(MockProvider.EXAMPLE_TASK_DEFINITION_KEY)).body("query.description", equalTo(MockProvider.EXAMPLE_TASK_DESCRIPTION)).body("query.descriptionLike", equalTo(MockProvider.EXAMPLE_TASK_DESCRIPTION)).body("query.involvedUser", equalTo(MockProvider.EXAMPLE_USER_ID)).body("query.priority", equalTo(1)).body("query.maxPriority", equalTo(2)).body("query.minPriority", equalTo(3)).body("query.name", equalTo(MockProvider.EXAMPLE_TASK_NAME)).body("query.nameLike", equalTo(MockProvider.EXAMPLE_TASK_NAME)).body("query.owner", equalTo(MockProvider.EXAMPLE_TASK_OWNER)).body("query.parentTaskId", equalTo(MockProvider.EXAMPLE_TASK_PARENT_TASK_ID)).body("query.tenantIdIn", hasItems(MockProvider.EXAMPLE_TENANT_ID, MockProvider.ANOTHER_EXAMPLE_TENANT_ID)).when().get(SINGLE_FILTER_URL);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithCandidateGroupQuery()
	  public virtual void testGetFilterWithCandidateGroupQuery()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateGroup("abc");
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.candidateGroup", equalTo("abc")).body("query.containsKey('candidateGroups')", @is(false)).body("query.containsKey('includeAssignedTasks')", @is(false)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithCandidateUserQuery()
	  public virtual void testGetFilterWithCandidateUserQuery()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateUser("abc");
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.candidateUser", equalTo("abc")).body("query.containsKey('candidateGroups')", @is(false)).body("query.containsKey('includeAssignedTasks')", @is(false)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithCandidateIncludeAssignedTasks()
	  public virtual void testGetFilterWithCandidateIncludeAssignedTasks()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.taskCandidateUser("abc").includeAssignedTasks();
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.candidateUser", equalTo("abc")).body("query.containsKey('candidateGroups')", @is(false)).body("query.includeAssignedTasks", @is(true)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithoutTenantIdQuery()
	  public virtual void testGetFilterWithoutTenantIdQuery()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.withoutTenantId();
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.withoutTenantId", @is(true)).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithoutSorting()
	  public virtual void testGetFilterWithoutSorting()
	  {
		TaskQuery query = new TaskQueryImpl();
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.sorting", EmptyOrNullString).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithSingleSorting()
	  public virtual void testGetFilterWithSingleSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByTaskName().desc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(1);
		assertSorting(sortings[0], SORT_BY_NAME_VALUE, SORT_ORDER_DESC_VALUE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithMultipleSorting()
	  public virtual void testGetFilterWithMultipleSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByDueDate().asc().orderByCaseExecutionId().desc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(2);
		assertSorting(sortings[0], SORT_BY_DUE_DATE_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[1], SORT_BY_CASE_EXECUTION_ID_VALUE, SORT_ORDER_DESC_VALUE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithAllPropertiesSorting()
	  public virtual void testGetFilterWithAllPropertiesSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByProcessInstanceId().asc().orderByCaseInstanceId().asc().orderByDueDate().asc().orderByFollowUpDate().asc().orderByExecutionId().asc().orderByCaseExecutionId().asc().orderByTaskAssignee().asc().orderByTaskCreateTime().asc().orderByTaskDescription().asc().orderByTaskId().asc().orderByTaskName().asc().orderByTaskNameCaseInsensitive().asc().orderByTaskPriority().asc().orderByTenantId().asc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(14);
		assertSorting(sortings[0], SORT_BY_PROCESS_INSTANCE_ID_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[1], SORT_BY_CASE_INSTANCE_ID_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[2], SORT_BY_DUE_DATE_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[3], SORT_BY_FOLLOW_UP_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[4], SORT_BY_EXECUTION_ID_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[5], SORT_BY_CASE_EXECUTION_ID_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[6], SORT_BY_ASSIGNEE_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[7], SORT_BY_CREATE_TIME_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[8], SORT_BY_DESCRIPTION_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[9], SORT_BY_ID_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[10], SORT_BY_NAME_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[11], SORT_BY_NAME_CASE_INSENSITIVE_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[12], SORT_BY_PRIORITY_VALUE, SORT_ORDER_ASC_VALUE);
		assertSorting(sortings[13], SORT_BY_TENANT_ID_VALUE, SORT_ORDER_ASC_VALUE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithVariableTypeSorting()
	  public virtual void testGetFilterWithVariableTypeSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByExecutionVariable("foo", ValueType.STRING).asc().orderByProcessVariable("foo", ValueType.STRING).asc().orderByTaskVariable("foo", ValueType.STRING).asc().orderByCaseExecutionVariable("foo", ValueType.STRING).asc().orderByCaseInstanceVariable("foo", ValueType.STRING).asc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(5);
		assertSorting(sortings[0], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[1], SORT_BY_PROCESS_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[2], SORT_BY_TASK_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[3], SORT_BY_CASE_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[4], SORT_BY_CASE_INSTANCE_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithVariableValueTypeSorting()
	  public virtual void testGetFilterWithVariableValueTypeSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByExecutionVariable("foo", ValueType.STRING).asc().orderByExecutionVariable("foo", ValueType.INTEGER).asc().orderByExecutionVariable("foo", ValueType.SHORT).asc().orderByExecutionVariable("foo", ValueType.DATE).asc().orderByExecutionVariable("foo", ValueType.BOOLEAN).asc().orderByExecutionVariable("foo", ValueType.LONG).asc().orderByExecutionVariable("foo", ValueType.DOUBLE).asc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(7);
		assertSorting(sortings[0], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[1], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.INTEGER);
		assertSorting(sortings[2], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.SHORT);
		assertSorting(sortings[3], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.DATE);
		assertSorting(sortings[4], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.BOOLEAN);
		assertSorting(sortings[5], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.LONG);
		assertSorting(sortings[6], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.DOUBLE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithVariableSortOrderSorting()
	  public virtual void testGetFilterWithVariableSortOrderSorting()
	  {
		TaskQuery query = (new TaskQueryImpl()).orderByExecutionVariable("foo", ValueType.STRING).asc().orderByExecutionVariable("foo", ValueType.STRING).desc();

		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(SINGLE_FILTER_URL);

		// validate sorting content
		string content = response.asString();
		IList<IDictionary<string, object>> sortings = from(content).getJsonObject("query.sorting");
		assertThat(sortings).hasSize(2);
		assertSorting(sortings[0], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_ASC_VALUE, "foo", ValueType.STRING);
		assertSorting(sortings[1], SORT_BY_EXECUTION_VARIABLE, SORT_ORDER_DESC_VALUE, "foo", ValueType.STRING);
	  }

	  protected internal virtual void assertSorting(IDictionary<string, object> sorting, string sortBy, string sortOrder)
	  {
		assertSorting(sorting, sortBy, sortOrder, null, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void assertSorting(java.util.Map<String, Object> sorting, String sortBy, String sortOrder, String parametersVariable, org.camunda.bpm.engine.variable.type.ValueType parametersType)
	  protected internal virtual void assertSorting(IDictionary<string, object> sorting, string sortBy, string sortOrder, string parametersVariable, ValueType parametersType)
	  {
		assertThat(sorting["sortBy"]).isEqualTo(sortBy);
		assertThat(sorting["sortOrder"]).isEqualTo(sortOrder);
		if (string.ReferenceEquals(parametersVariable, null))
		{
		  assertThat(sorting.ContainsKey("parameters")).False;
		}
		else
		{
		  IDictionary<string, object> parameters = (IDictionary<string, object>) sorting["parameters"];
		  assertThat(parameters["variable"]).isEqualTo(parametersVariable);
		  assertThat(parameters["type"]).isEqualTo(VariableValueDto.toRestApiTypeName(parametersType.Name));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetFilterWithFollowUpBeforeOrNotExistentExpression()
	  public virtual void testGetFilterWithFollowUpBeforeOrNotExistentExpression()
	  {
		TaskQueryImpl query = new TaskQueryImpl();
		query.followUpBeforeOrNotExistentExpression("#{now()}");
		Filter filter = (new FilterEntity("Task")).setName("test").setQuery(query);
		when(filterServiceMock.getFilter(EXAMPLE_FILTER_ID)).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("query.followUpBeforeOrNotExistentExpression", equalTo("#{now()}")).when().get(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateFilter()
	  public virtual void testCreateFilter()
	  {
		IDictionary<string, object> json = toFilterRequest(MockProvider.createMockFilter());

		given().contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.OK.StatusCode).body("id", notNullValue()).when().post(CREATE_FILTER_URL);

		verify(filterServiceMock).newTaskFilter();
		verify(filterServiceMock).saveFilter(eq(filterMock));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateInvalidFilter()
	  public virtual void testCreateInvalidFilter()
	  {
		given().contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(CREATE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateFilter()
	  public virtual void testUpdateFilter()
	  {
		IDictionary<string, object> json = toFilterRequest(MockProvider.createMockFilter());

		given().pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().put(SINGLE_FILTER_URL);

		verify(filterServiceMock).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(filterServiceMock).saveFilter(eq(filterMock));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidResourceType()
	  public virtual void testInvalidResourceType()
	  {
		IDictionary<string, object> json = toFilterRequest(MockProvider.createMockFilter());
		json["resourceType"] = "invalid";

		given().pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(json).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().put(SINGLE_FILTER_URL);
	  }

	  protected internal virtual IDictionary<string, object> toFilterRequest(Filter filter)
	  {
		IDictionary<string, object> json = new Dictionary<string, object>();

		json["id"] = filter.Id;
		json["name"] = filter.Name;
		json["owner"] = filter.Owner;
		json["properties"] = filter.Properties;
		json["resourceType"] = filter.ResourceType;

		// should not use the dto classes in client-side tests
		json["query"] = TaskQueryDto.fromQuery(filter.Query);

		return json;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateNonExistingFilter()
	  public virtual void testUpdateNonExistingFilter()
	  {
		given().pathParam("id", MockProvider.NON_EXISTING_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().put(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateInvalidFilter()
	  public virtual void testUpdateInvalidFilter()
	  {
		given().pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().put(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteFilter()
	  public virtual void testDeleteFilter()
	  {
		given().pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().delete(SINGLE_FILTER_URL);

		verify(filterServiceMock).deleteFilter(eq(EXAMPLE_FILTER_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteNonExistingFilter()
	  public virtual void testDeleteNonExistingFilter()
	  {
		given().pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().delete(SINGLE_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResult()
	  public virtual void testExecuteSingleResult()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptySingleResult()
	  public virtual void testEmptySingleResult()
	  {
		when(filterServiceMock.singleResult(anyString(), any(typeof(Query)))).thenReturn(null);

		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.NO_CONTENT.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidSingleResult()
	  public virtual void testInvalidSingleResult()
	  {
		doThrow(new ProcessEngineException("More than one result found")).when(filterServiceMock).singleResult(anyString(), any(typeof(Query)));

		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteHalSingleResult()
	  public virtual void testExecuteHalSingleResult()
	  {
		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID),isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyHalSingleResult()
	  public virtual void testEmptyHalSingleResult()
	  {
		when(filterServiceMock.singleResult(anyString(), any(typeof(Query)))).thenReturn(null);

		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("_links.size()", equalTo(0)).body("_embedded.size()", equalTo(0)).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidHalSingleResult()
	  public virtual void testInvalidHalSingleResult()
	  {
		doThrow(new ProcessEngineException("More than one result found")).when(filterServiceMock).singleResult(anyString(), any(typeof(Query)));

		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResultOfNonExistingFilter()
	  public virtual void testExecuteSingleResultOfNonExistingFilter()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(MockProvider.NON_EXISTING_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResultAsPost()
	  public virtual void testExecuteSingleResultAsPost()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), any(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteHalSingleResultAsPost()
	  public virtual void testExecuteHalSingleResultAsPost()
	  {
		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), any(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResultInvalidWithExtendingQuery()
	  public virtual void testExecuteSingleResultInvalidWithExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(invalidExtendingQuery).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_SINGLE_RESULT_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResultWithExtendingQuery()
	  public virtual void testExecuteSingleResultWithExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(extendingQueryDto).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), argThat(hasName(MockProvider.EXAMPLE_TASK_NAME)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteSingleResultWithExtendingOrQuery()
	  public virtual void testExecuteSingleResultWithExtendingOrQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(extendingOrQueryDto).then().expect().statusCode(Status.OK.StatusCode).when().post(EXECUTE_SINGLE_RESULT_FILTER_URL);

		ArgumentCaptor<TaskQueryImpl> argument = ArgumentCaptor.forClass(typeof(TaskQueryImpl));
		verify(filterServiceMock).singleResult(eq(EXAMPLE_FILTER_ID), argument.capture());
		assertEquals(MockProvider.EXAMPLE_TASK_DESCRIPTION, argument.Value.Queries.get(1).Description);
		assertEquals(MockProvider.EXAMPLE_TASK_NAME, argument.Value.Queries.get(2).Name);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteList()
	  public virtual void testExecuteList()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyList()
	  public virtual void testEmptyList()
	  {
		when(filterServiceMock.list(anyString(), any(typeof(Query)))).thenReturn(Collections.emptyList());

		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(0)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteHalList()
	  public virtual void testExecuteHalList()
	  {
		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyHalList()
	  public virtual void testEmptyHalList()
	  {
		when(filterServiceMock.list(anyString(), any(typeof(Query)))).thenReturn(Collections.emptyList());
		when(filterServiceMock.count(anyString(), any(typeof(Query)))).thenReturn(0l);

		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("_links.size()", equalTo(0)).body("_embedded.size()", equalTo(0)).body("count", equalTo(0)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListOfNonExistingFilter()
	  public virtual void testExecuteListOfNonExistingFilter()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(MockProvider.NON_EXISTING_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListWithPagination()
	  public virtual void testExecuteListWithPagination()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).queryParams("firstResult", 1).queryParams("maxResults", 2).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).listPage(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)), eq(1), eq(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListAsPost()
	  public virtual void testExecuteListAsPost()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().post(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), any(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteHalListAsPost()
	  public virtual void testExecuteHalListAsPost()
	  {
		given().header(ACCEPT_HAL_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), any(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListAsPostWithPagination()
	  public virtual void testExecuteListAsPostWithPagination()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).queryParams("firstResult", 1).queryParams("maxResults", 2).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().post(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).listPage(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)), eq(1), eq(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListWithExtendingQuery()
	  public virtual void testExecuteListWithExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(extendingQueryDto).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().post(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).list(eq(EXAMPLE_FILTER_ID), argThat(hasName(MockProvider.EXAMPLE_TASK_NAME)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListWithExtendingQueryWithPagination()
	  public virtual void testExecuteListWithExtendingQueryWithPagination()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).queryParams("firstResult", 1).queryParams("maxResults", 2).contentType(POST_JSON_CONTENT_TYPE).body(extendingQueryDto).then().expect().statusCode(Status.OK.StatusCode).body("$.size()", equalTo(1)).when().post(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock).listPage(eq(EXAMPLE_FILTER_ID), argThat(hasName(MockProvider.EXAMPLE_TASK_NAME)), eq(1), eq(2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteListWithInvalidExtendingQuery()
	  public virtual void testExecuteListWithInvalidExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(invalidExtendingQuery).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_LIST_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteCount()
	  public virtual void testExecuteCount()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().get(EXECUTE_COUNT_FILTER_URL);

		verify(filterServiceMock).count(eq(EXAMPLE_FILTER_ID), isNull(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteCountOfNonExistingFilter()
	  public virtual void testExecuteCountOfNonExistingFilter()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", MockProvider.NON_EXISTING_ID).then().expect().statusCode(Status.NOT_FOUND.StatusCode).when().get(EXECUTE_COUNT_FILTER_URL);

		verify(filterServiceMock).count(MockProvider.NON_EXISTING_ID, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteCountAsPost()
	  public virtual void testExecuteCountAsPost()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(EMPTY_JSON_OBJECT).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(EXECUTE_COUNT_FILTER_URL);

		verify(filterServiceMock).count(eq(EXAMPLE_FILTER_ID), any(typeof(Query)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteCountWithExtendingQuery()
	  public virtual void testExecuteCountWithExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(extendingQueryDto).then().expect().statusCode(Status.OK.StatusCode).body("count", equalTo(1)).when().post(EXECUTE_COUNT_FILTER_URL);

		verify(filterServiceMock).count(eq(EXAMPLE_FILTER_ID), argThat(hasName(MockProvider.EXAMPLE_TASK_NAME)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExecuteCountWithInvalidExtendingQuery()
	  public virtual void testExecuteCountWithInvalidExtendingQuery()
	  {
		given().header(ACCEPT_JSON_HEADER).pathParam("id", EXAMPLE_FILTER_ID).contentType(POST_JSON_CONTENT_TYPE).body(invalidExtendingQuery).then().expect().statusCode(Status.BAD_REQUEST.StatusCode).when().post(EXECUTE_COUNT_FILTER_URL);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAnonymousFilterOptions()
	  public virtual void testAnonymousFilterOptions()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL;

		// anonymity means the identityService returns a null authentication, so no need to mock here

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links.size()", @is(3)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullFilterUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullFilterUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(FILTER_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRestrictedFilterOptions()
	  public virtual void testRestrictedFilterOptions()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, FILTER, ANY)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().then().statusCode(Status.OK.StatusCode).body("links.size()", @is(2)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullFilterUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).when().options(FILTER_URL);

		verify(identityServiceMock, times(1)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterOptionsWithDisabledAuthorization()
	  public virtual void testFilterOptionsWithDisabledAuthorization()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().then().statusCode(Status.OK.StatusCode).body("links.size()", @is(3)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("list")).body("links[1].href", equalTo(fullFilterUrl + "/count")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("count")).body("links[2].href", equalTo(fullFilterUrl + "/create")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("create")).when().options(FILTER_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAnonymousFilterResourceOptions()
	  public virtual void testAnonymousFilterResourceOptions()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL + "/" + EXAMPLE_FILTER_ID;

		// anonymity means the identityService returns a null authentication, so no need to mock here

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().statusCode(Status.OK.StatusCode).body("links.size()", @is(9)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullFilterUrl + "/singleResult")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("singleResult")).body("links[2].href", equalTo(fullFilterUrl + "/singleResult")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("singleResult")).body("links[3].href", equalTo(fullFilterUrl + "/list")).body("links[3].method", equalTo(HttpMethod.GET)).body("links[3].rel", equalTo("list")).body("links[4].href", equalTo(fullFilterUrl + "/list")).body("links[4].method", equalTo(HttpMethod.POST)).body("links[4].rel", equalTo("list")).body("links[5].href", equalTo(fullFilterUrl + "/count")).body("links[5].method", equalTo(HttpMethod.GET)).body("links[5].rel", equalTo("count")).body("links[6].href", equalTo(fullFilterUrl + "/count")).body("links[6].method", equalTo(HttpMethod.POST)).body("links[6].rel", equalTo("count")).body("links[7].href", equalTo(fullFilterUrl)).body("links[7].method", equalTo(HttpMethod.DELETE)).body("links[7].rel", equalTo("delete")).body("links[8].href", equalTo(fullFilterUrl)).body("links[8].method", equalTo(HttpMethod.PUT)).body("links[8].rel", equalTo("update")).when().options(SINGLE_FILTER_URL);

		verify(identityServiceMock, times(3)).CurrentAuthentication;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterResourceOptionsUnauthorized()
	  public virtual void testFilterResourceOptionsUnauthorized()
	  {
		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);

		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, READ, FILTER, EXAMPLE_FILTER_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, FILTER, EXAMPLE_FILTER_ID)).thenReturn(false);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, FILTER, EXAMPLE_FILTER_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().statusCode(Status.OK.StatusCode).body("links.size()", @is(0)).when().options(SINGLE_FILTER_URL);

		verify(identityServiceMock, times(3)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, READ, FILTER, EXAMPLE_FILTER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, FILTER, EXAMPLE_FILTER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, FILTER, EXAMPLE_FILTER_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterResourceOptionsUpdateUnauthorized()
	  public virtual void testFilterResourceOptionsUpdateUnauthorized()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL + "/" + EXAMPLE_FILTER_ID;

		Authentication authentication = new Authentication(MockProvider.EXAMPLE_USER_ID, null);
		when(identityServiceMock.CurrentAuthentication).thenReturn(authentication);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, READ, FILTER, EXAMPLE_FILTER_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, FILTER, EXAMPLE_FILTER_ID)).thenReturn(true);
		when(authorizationServiceMock.isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, FILTER, EXAMPLE_FILTER_ID)).thenReturn(false);

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(true);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().statusCode(Status.OK.StatusCode).body("links.size()", @is(8)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullFilterUrl + "/singleResult")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("singleResult")).body("links[2].href", equalTo(fullFilterUrl + "/singleResult")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("singleResult")).body("links[3].href", equalTo(fullFilterUrl + "/list")).body("links[3].method", equalTo(HttpMethod.GET)).body("links[3].rel", equalTo("list")).body("links[4].href", equalTo(fullFilterUrl + "/list")).body("links[4].method", equalTo(HttpMethod.POST)).body("links[4].rel", equalTo("list")).body("links[5].href", equalTo(fullFilterUrl + "/count")).body("links[5].method", equalTo(HttpMethod.GET)).body("links[5].rel", equalTo("count")).body("links[6].href", equalTo(fullFilterUrl + "/count")).body("links[6].method", equalTo(HttpMethod.POST)).body("links[6].rel", equalTo("count")).body("links[7].href", equalTo(fullFilterUrl)).body("links[7].method", equalTo(HttpMethod.DELETE)).body("links[7].rel", equalTo("delete")).when().options(SINGLE_FILTER_URL);

		verify(identityServiceMock, times(3)).CurrentAuthentication;
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, READ, FILTER, EXAMPLE_FILTER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, DELETE, FILTER, EXAMPLE_FILTER_ID);
		verify(authorizationServiceMock, times(1)).isUserAuthorized(MockProvider.EXAMPLE_USER_ID, null, UPDATE, FILTER, EXAMPLE_FILTER_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterResourceOptionsWithAuthorizationDisabled()
	  public virtual void testFilterResourceOptionsWithAuthorizationDisabled()
	  {
		string fullFilterUrl = "http://localhost:" + PORT + FILTER_URL + "/" + EXAMPLE_FILTER_ID;

		when(processEngineConfigurationMock.AuthorizationEnabled).thenReturn(false);

		given().pathParam("id", EXAMPLE_FILTER_ID).then().statusCode(Status.OK.StatusCode).body("links.size()", @is(9)).body("links[0].href", equalTo(fullFilterUrl)).body("links[0].method", equalTo(HttpMethod.GET)).body("links[0].rel", equalTo("self")).body("links[1].href", equalTo(fullFilterUrl + "/singleResult")).body("links[1].method", equalTo(HttpMethod.GET)).body("links[1].rel", equalTo("singleResult")).body("links[2].href", equalTo(fullFilterUrl + "/singleResult")).body("links[2].method", equalTo(HttpMethod.POST)).body("links[2].rel", equalTo("singleResult")).body("links[3].href", equalTo(fullFilterUrl + "/list")).body("links[3].method", equalTo(HttpMethod.GET)).body("links[3].rel", equalTo("list")).body("links[4].href", equalTo(fullFilterUrl + "/list")).body("links[4].method", equalTo(HttpMethod.POST)).body("links[4].rel", equalTo("list")).body("links[5].href", equalTo(fullFilterUrl + "/count")).body("links[5].method", equalTo(HttpMethod.GET)).body("links[5].rel", equalTo("count")).body("links[6].href", equalTo(fullFilterUrl + "/count")).body("links[6].method", equalTo(HttpMethod.POST)).body("links[6].rel", equalTo("count")).body("links[7].href", equalTo(fullFilterUrl)).body("links[7].method", equalTo(HttpMethod.DELETE)).body("links[7].rel", equalTo("delete")).body("links[8].href", equalTo(fullFilterUrl)).body("links[8].method", equalTo(HttpMethod.PUT)).body("links[8].rel", equalTo("update")).when().options(SINGLE_FILTER_URL);

		verifyNoAuthorizationCheckPerformed();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskQueryWithWrongFormatVariablesProperties()
	  public virtual void testHalTaskQueryWithWrongFormatVariablesProperties()
	  {
		// mock properties with variable name list in wrong format
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["variables"] = "foo";
		Filter filter = mockFilter().properties(properties).build();
		when(filterServiceMock.getFilter(eq(EXAMPLE_FILTER_ID))).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).expect().statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock, times(1)).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(variableInstanceQueryMock, never()).variableScopeIdIn((string) anyVararg());
		verify(variableInstanceQueryMock, never()).variableNameIn((string) anyVararg());
		verify(variableInstanceQueryMock, never()).list();

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskQueryWithEmptyVariablesProperties()
	  public virtual void testHalTaskQueryWithEmptyVariablesProperties()
	  {
		// mock properties with variable name list in wrong format
		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["variables"] = Collections.emptyList();
		Filter filter = mockFilter().properties(properties).build();
		when(filterServiceMock.getFilter(eq(EXAMPLE_FILTER_ID))).thenReturn(filter);

		given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).expect().statusCode(Status.OK.StatusCode).body("_embedded", equalTo(null)).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock, times(1)).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(variableInstanceQueryMock, never()).variableScopeIdIn((string) anyVararg());
		verify(variableInstanceQueryMock, never()).variableNameIn((string) anyVararg());
		verify(variableInstanceQueryMock, never()).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskQueryWithVariableNotSetOnTask()
	  public virtual void testHalTaskQueryWithVariableNotSetOnTask()
	  {
		// mock filter with variable names set
		mockFilterWithVariableNames();

		// mock resulting task
		Task task = createTaskMock(TASK_A_ID, PROCESS_INSTANCE_A_ID, EXECUTION_A_ID, null, null);
		when(filterServiceMock.singleResult(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(task);

		// mock variable instances
		IList<VariableInstance> variableInstances = Arrays.asList(createExecutionVariableInstanceMock("foo", stringValue("execution"), EXECUTION_B_ID), createExecutionVariableInstanceMock("execution", stringValue("bar"), EXECUTION_B_ID), createTaskVariableInstanceMock("foo", stringValue("task"), TASK_B_ID), createTaskVariableInstanceMock("task", stringValue("bar"), TASK_B_ID));
		when(variableInstanceQueryMock.list()).thenReturn(variableInstances);

		given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).expect().statusCode(Status.OK.StatusCode).body("_embedded.containsKey('variable')", @is(true)).body("_embedded.variable.size", equalTo(0)).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock, times(1)).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(variableInstanceQueryMock, times(1)).variableScopeIdIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableScopeIdIn(TASK_A_ID, EXECUTION_A_ID, PROCESS_INSTANCE_A_ID);
		verify(variableInstanceQueryMock, times(1)).variableNameIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableNameIn("foo", "bar");
		verify(variableInstanceQueryMock, times(1)).list();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskQueryWithAdditionalVariables()
	  public virtual void testHalTaskQueryWithAdditionalVariables()
	  {
		// mock filter with variable names set
		mockFilterWithVariableNames();

		// mock resulting task
		Task task = createTaskMock(TASK_A_ID, PROCESS_INSTANCE_A_ID, EXECUTION_A_ID, CASE_INSTANCE_A_ID, CASE_EXECUTION_A_ID);
		when(filterServiceMock.singleResult(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(task);

		// mock variable instances
		IList<VariableInstance> variableInstances = Arrays.asList(createProcessInstanceVariableInstanceMock("foo", stringValue("processInstance"), PROCESS_INSTANCE_A_ID), createProcessInstanceVariableInstanceMock("processInstance", stringValue("bar"), PROCESS_INSTANCE_A_ID), createExecutionVariableInstanceMock("foo", stringValue("execution"), EXECUTION_A_ID), createExecutionVariableInstanceMock("execution", stringValue("bar"), EXECUTION_A_ID), createTaskVariableInstanceMock("foo", stringValue("task"), TASK_A_ID), createTaskVariableInstanceMock("task", stringValue("bar"), TASK_A_ID), createCaseInstanceVariableInstanceMock("foo", stringValue("caseInstance"), CASE_INSTANCE_A_ID), createCaseInstanceVariableInstanceMock("caseInstance", stringValue("bar"), CASE_INSTANCE_A_ID), createCaseExecutionVariableInstanceMock("foo", stringValue("caseExecution"), CASE_EXECUTION_A_ID), createCaseExecutionVariableInstanceMock("caseExecution", stringValue("bar"), CASE_EXECUTION_A_ID));
		when(variableInstanceQueryMock.list()).thenReturn(variableInstances);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).expect().statusCode(Status.OK.StatusCode).body("_embedded.containsKey('variable')", @is(true)).body("_embedded.variable.size", equalTo(6)).when().get(EXECUTE_SINGLE_RESULT_FILTER_URL);

		verify(filterServiceMock, times(1)).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(variableInstanceQueryMock, times(1)).variableScopeIdIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableScopeIdIn(TASK_A_ID, EXECUTION_A_ID, PROCESS_INSTANCE_A_ID, CASE_EXECUTION_A_ID, CASE_INSTANCE_A_ID);
		verify(variableInstanceQueryMock, times(1)).variableNameIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableNameIn("foo", "bar");
		verify(variableInstanceQueryMock, times(1)).list();
		verify(variableInstanceQueryMock, times(1)).disableBinaryFetching();
		verify(variableInstanceQueryMock, times(1)).disableCustomObjectDeserialization();

		string content = response.asString();
		IList<IDictionary<string, object>> variables = from(content).getJsonObject("_embedded.variable");

		// task variable 'foo'
		verifyTaskVariableValue(variables[0], "foo", "task", TASK_A_ID);
		// task variable 'task'
		verifyTaskVariableValue(variables[1], "task", "bar", TASK_A_ID);
		// execution variable 'execution'
		verifyExecutionVariableValue(variables[2], "execution", "bar", EXECUTION_A_ID);
		// process instance variable 'processInstance'
		verifyProcessInstanceVariableValue(variables[3], "processInstance", "bar", PROCESS_INSTANCE_A_ID);
		// caseExecution variable 'caseExecution'
		verifyCaseExecutionVariableValue(variables[4], "caseExecution", "bar", CASE_EXECUTION_A_ID);
		// case instance variable 'caseInstance'
		verifyCaseInstanceVariableValue(variables[5], "caseInstance", "bar", CASE_INSTANCE_A_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskListQueryWithAdditionalVariables()
	  public virtual void testHalTaskListQueryWithAdditionalVariables()
	  {
		// mock filter with variable names set
		mockFilterWithVariableNames();

		// mock resulting task
		IList<Task> tasks = Arrays.asList(createTaskMock(TASK_A_ID, PROCESS_INSTANCE_A_ID, EXECUTION_A_ID, null, null), createTaskMock(TASK_B_ID, PROCESS_INSTANCE_A_ID, EXECUTION_B_ID, null, null), createTaskMock(TASK_C_ID, null, null, CASE_INSTANCE_A_ID, CASE_EXECUTION_A_ID));
		when(filterServiceMock.list(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(tasks);

		// mock variable instances
		IList<VariableInstance> variableInstances = Arrays.asList(createProcessInstanceVariableInstanceMock("foo", stringValue(PROCESS_INSTANCE_A_ID), PROCESS_INSTANCE_A_ID), createProcessInstanceVariableInstanceMock(PROCESS_INSTANCE_A_ID, stringValue("bar"), PROCESS_INSTANCE_A_ID), createExecutionVariableInstanceMock("foo", stringValue(EXECUTION_A_ID), EXECUTION_A_ID), createExecutionVariableInstanceMock(EXECUTION_A_ID, stringValue("bar"), EXECUTION_A_ID), createExecutionVariableInstanceMock("foo", stringValue(EXECUTION_B_ID), EXECUTION_B_ID), createExecutionVariableInstanceMock(EXECUTION_B_ID, stringValue("bar"), EXECUTION_B_ID), createTaskVariableInstanceMock("foo", stringValue(TASK_A_ID), TASK_A_ID), createTaskVariableInstanceMock(TASK_A_ID, stringValue("bar"), TASK_A_ID), createTaskVariableInstanceMock("foo", stringValue(TASK_B_ID), TASK_B_ID), createTaskVariableInstanceMock(TASK_B_ID, stringValue("bar"), TASK_B_ID), createTaskVariableInstanceMock("foo", stringValue(TASK_C_ID), TASK_C_ID), createTaskVariableInstanceMock(TASK_C_ID, stringValue("bar"), TASK_C_ID), createCaseInstanceVariableInstanceMock("foo", stringValue(CASE_INSTANCE_A_ID), CASE_INSTANCE_A_ID), createCaseInstanceVariableInstanceMock(CASE_INSTANCE_A_ID, stringValue("bar"), CASE_INSTANCE_A_ID), createCaseExecutionVariableInstanceMock("foo", stringValue(CASE_EXECUTION_A_ID), CASE_EXECUTION_A_ID), createCaseExecutionVariableInstanceMock(CASE_EXECUTION_A_ID, stringValue("bar"), CASE_EXECUTION_A_ID));
		when(variableInstanceQueryMock.list()).thenReturn(variableInstances);

		Response response = given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).expect().statusCode(Status.OK.StatusCode).body("_embedded.task.size", equalTo(3)).body("_embedded.task.any { it._embedded.containsKey('variable') }", @is(true)).when().get(EXECUTE_LIST_FILTER_URL);

		verify(filterServiceMock, times(1)).getFilter(eq(EXAMPLE_FILTER_ID));
		verify(variableInstanceQueryMock, times(1)).variableScopeIdIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableScopeIdIn(TASK_A_ID, EXECUTION_A_ID, PROCESS_INSTANCE_A_ID, TASK_B_ID, EXECUTION_B_ID, TASK_C_ID, CASE_EXECUTION_A_ID, CASE_INSTANCE_A_ID);
		verify(variableInstanceQueryMock, times(1)).variableNameIn((string) anyVararg());
		verify(variableInstanceQueryMock).variableNameIn("foo", "bar");
		verify(variableInstanceQueryMock, times(1)).list();

		string content = response.asString();
		IList<IDictionary<string, object>> taskList = from(content).getList("_embedded.task");

		// task A
		IList<IDictionary<string, object>> variables = getEmbeddedTaskVariables(taskList[0]);
		assertThat(variables).hasSize(4);

		// task variable 'foo'
		verifyTaskVariableValue(variables[0], "foo", TASK_A_ID, TASK_A_ID);
		// task variable 'taskA'
		verifyTaskVariableValue(variables[1], TASK_A_ID, "bar", TASK_A_ID);
		// execution variable 'executionA'
		verifyExecutionVariableValue(variables[2], EXECUTION_A_ID, "bar", EXECUTION_A_ID);
		// process instance variable 'processInstanceA'
		verifyProcessInstanceVariableValue(variables[3], PROCESS_INSTANCE_A_ID, "bar", PROCESS_INSTANCE_A_ID);

		// task B
		variables = getEmbeddedTaskVariables(taskList[1]);
		assertThat(variables).hasSize(4);

		// task variable 'foo'
		verifyTaskVariableValue(variables[0], "foo", TASK_B_ID, TASK_B_ID);
		// task variable 'taskA'
		verifyTaskVariableValue(variables[1], TASK_B_ID, "bar", TASK_B_ID);
		// execution variable 'executionA'
		verifyExecutionVariableValue(variables[2], EXECUTION_B_ID, "bar", EXECUTION_B_ID);
		// process instance variable 'processInstanceA'
		verifyProcessInstanceVariableValue(variables[3], PROCESS_INSTANCE_A_ID, "bar", PROCESS_INSTANCE_A_ID);

		// task C
		variables = getEmbeddedTaskVariables(taskList[2]);
		assertThat(variables).hasSize(4);

		// task variable 'foo'
		verifyTaskVariableValue(variables[0], "foo", TASK_C_ID, TASK_C_ID);
		// task variable 'taskC'
		verifyTaskVariableValue(variables[1], TASK_C_ID, "bar", TASK_C_ID);
		// case execution variable 'caseExecutionA'
		verifyCaseExecutionVariableValue(variables[2], CASE_EXECUTION_A_ID, "bar", CASE_EXECUTION_A_ID);
		// case instance variable 'caseInstanceA'
		verifyCaseInstanceVariableValue(variables[3], CASE_INSTANCE_A_ID, "bar", CASE_INSTANCE_A_ID);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testHalTaskListCount()
	  public virtual void testHalTaskListCount()
	  {
		// mock resulting task
		IList<Task> tasks = Arrays.asList(createTaskMock(TASK_A_ID, PROCESS_INSTANCE_A_ID, EXECUTION_A_ID, null, null), createTaskMock(TASK_B_ID, PROCESS_INSTANCE_A_ID, EXECUTION_A_ID, null, null), createTaskMock(TASK_C_ID, PROCESS_INSTANCE_A_ID, EXECUTION_B_ID, null, null));
		when(filterServiceMock.list(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn(tasks);
		when(filterServiceMock.listPage(eq(EXAMPLE_FILTER_ID), any(typeof(Query)), eq(0), eq(2))).thenReturn(tasks.subList(0, 2));
		when(filterServiceMock.listPage(eq(EXAMPLE_FILTER_ID), any(typeof(Query)), eq(5), eq(2))).thenReturn(Collections.emptyList());
		when(filterServiceMock.count(eq(EXAMPLE_FILTER_ID), any(typeof(Query)))).thenReturn((long) tasks.Count);

		given().pathParam("id", EXAMPLE_FILTER_ID).header(ACCEPT_HAL_HEADER).then().expect().body("_embedded.task.size", equalTo(3)).body("count", equalTo(3)).when().get(EXECUTE_LIST_FILTER_URL);

		given().pathParam("id", EXAMPLE_FILTER_ID).queryParam("firstResult", 0).queryParam("maxResults", 2).header(ACCEPT_HAL_HEADER).then().expect().body("_embedded.task.size", equalTo(2)).body("count", equalTo(3)).when().get(EXECUTE_LIST_FILTER_URL);

		given().pathParam("id", EXAMPLE_FILTER_ID).queryParam("firstResult", 5).queryParam("maxResults", 2).header(ACCEPT_HAL_HEADER).then().expect().body("_embedded.containsKey('task')", @is(false)).body("count", equalTo(3)).when().get(EXECUTE_LIST_FILTER_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected java.util.List<java.util.Map<String, Object>> getEmbeddedTaskVariables(java.util.Map<String, Object> task)
	  protected internal virtual IList<IDictionary<string, object>> getEmbeddedTaskVariables(IDictionary<string, object> task)
	  {
		IDictionary<string, object> embedded = (IDictionary<string, object>) task["_embedded"];
		assertThat(embedded).NotNull;
		return (IList<IDictionary<string, object>>) embedded["variable"];
	  }

	  protected internal virtual Filter mockFilterWithVariableNames()
	  {
		// mock properties with variable name list (names are ignored but variable names list must not be empty)
		IList<IDictionary<string, string>> variables = new List<IDictionary<string, string>>();
		IDictionary<string, string> foo = new Dictionary<string, string>();
		foo["name"] = "foo";
		foo["label"] = "Foo";
		variables.Add(foo);
		IDictionary<string, string> bar = new Dictionary<string, string>();
		bar["name"] = "bar";
		bar["label"] = "Bar";
		variables.Add(bar);

		IDictionary<string, object> properties = new Dictionary<string, object>();
		properties["variables"] = variables;

		Filter filter = mockFilter().properties(properties).build();
		when(filterServiceMock.getFilter(eq(EXAMPLE_FILTER_ID))).thenReturn(filter);

		return filter;
	  }

	  protected internal virtual Task createTaskMock(string taskId, string processInstanceId, string executionId, string caseInstanceId, string caseExecutionId)
	  {
		return (new MockTaskBuilder()).id(taskId).processInstanceId(processInstanceId).executionId(executionId).caseInstanceId(caseInstanceId).caseExecutionId(caseExecutionId).build();
	  }

	  protected internal virtual VariableInstance createTaskVariableInstanceMock(string name, TypedValue value, string taskId)
	  {
		return createVariableInstanceMock(name, value, taskId, null, null, null, null);
	  }

	  protected internal virtual VariableInstance createExecutionVariableInstanceMock(string name, TypedValue value, string executionId)
	  {
		return createVariableInstanceMock(name, value, null, executionId, null, null, null);
	  }

	  protected internal virtual VariableInstance createProcessInstanceVariableInstanceMock(string name, TypedValue value, string processInstanceId)
	  {
		return createVariableInstanceMock(name, value, null, processInstanceId, processInstanceId, null, null);
	  }

	  protected internal virtual VariableInstance createCaseExecutionVariableInstanceMock(string name, TypedValue value, string caseExecutionId)
	  {
		return createVariableInstanceMock(name, value, null, null, null, caseExecutionId, null);
	  }

	  protected internal virtual VariableInstance createCaseInstanceVariableInstanceMock(string name, TypedValue value, string caseInstanceId)
	  {
		return createVariableInstanceMock(name, value, null, null, null, caseInstanceId, caseInstanceId);
	  }

	  protected internal virtual VariableInstance createVariableInstanceMock(string name, TypedValue value, string taskId, string executionId, string processInstanceId, string caseExecutionId, string caseInstanceId)
	  {
		return mockVariableInstance().name(name).typedValue(value).taskId(taskId).executionId(executionId).processInstanceId(processInstanceId).caseExecutionId(caseExecutionId).caseInstanceId(caseInstanceId).buildEntity();
	  }

	  protected internal virtual void verifyTaskVariableValue(IDictionary<string, object> variable, string name, string value, string taskId)
	  {
		verifyVariableValue(variable, name, value, TaskRestService_Fields.PATH, taskId, "localVariables");
	  }

	  protected internal virtual void verifyExecutionVariableValue(IDictionary<string, object> variable, string name, string value, string executionId)
	  {
		verifyVariableValue(variable, name, value, ExecutionRestService_Fields.PATH, executionId, "localVariables");
	  }

	  protected internal virtual void verifyCaseExecutionVariableValue(IDictionary<string, object> variable, string name, string value, string caseExecutionId)
	  {
		verifyVariableValue(variable, name, value, CaseExecutionRestService_Fields.PATH, caseExecutionId, "localVariables");
	  }

	  protected internal virtual void verifyProcessInstanceVariableValue(IDictionary<string, object> variable, string name, string value, string processInstanceId)
	  {
		verifyVariableValue(variable, name, value, ProcessInstanceRestService_Fields.PATH, processInstanceId, "variables");
	  }

	  protected internal virtual void verifyCaseInstanceVariableValue(IDictionary<string, object> variable, string name, string value, string caseInstanceId)
	  {
		verifyVariableValue(variable, name, value, CaseInstanceRestService_Fields.PATH, caseInstanceId, "variables");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") protected void verifyVariableValue(java.util.Map<String, Object> variable, String name, String value, String scopeResourcePath, String scopeId, String variablesName)
	  protected internal virtual void verifyVariableValue(IDictionary<string, object> variable, string name, string value, string scopeResourcePath, string scopeId, string variablesName)
	  {
		assertThat(variable["name"]).isEqualTo(name);
		assertThat(variable["value"]).isEqualTo(value);
		assertThat(variable["type"]).isEqualTo("String");
		assertThat(variable["valueInfo"]).isEqualTo(Collections.emptyMap());
		assertThat(variable["_embedded"]).Null;
		IDictionary<string, IDictionary<string, string>> links = (IDictionary<string, IDictionary<string, string>>) variable["_links"];
		assertThat(links).hasSize(1);
		assertThat(links["self"]).hasSize(1);
		assertThat(links["self"]["href"]).isEqualTo(scopeResourcePath + "/" + scopeId + "/" + variablesName + "/" + name);
	  }

	  protected internal virtual void verifyNoAuthorizationCheckPerformed()
	  {
		verify(identityServiceMock, times(0)).CurrentAuthentication;
		verify(authorizationServiceMock, times(0)).isUserAuthorized(anyString(), anyListOf(typeof(string)), any(typeof(Permission)), any(typeof(Resource)));
	  }

	}

}