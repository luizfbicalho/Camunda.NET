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
namespace org.camunda.bpm.engine.rest.impl
{
	using ContentType = io.restassured.http.ContentType;
	using ExternalTaskQueryTopicBuilder = org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Group = org.camunda.bpm.engine.identity.Group;
	using GroupQuery = org.camunda.bpm.engine.identity.GroupQuery;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using TenantQuery = org.camunda.bpm.engine.identity.TenantQuery;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using FetchExternalTasksExtendedDto = org.camunda.bpm.engine.rest.dto.externaltask.FetchExternalTasksExtendedDto;
	using InvalidRequestException = org.camunda.bpm.engine.rest.exception.InvalidRequestException;
	using MockProvider = org.camunda.bpm.engine.rest.helper.MockProvider;
	using TestContainerRule = org.camunda.bpm.engine.rest.util.container.TestContainerRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Ignore = org.junit.Ignore;
	using Test = org.junit.Test;
	using RunWith = org.junit.runner.RunWith;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;
	using InOrder = org.mockito.InOrder;
	using Mock = org.mockito.Mock;
	using MockitoJUnitRunner = org.mockito.runners.MockitoJUnitRunner;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static io.restassured.RestAssured.given;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsEqual.equalTo;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.IsNull.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyBoolean;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyInt;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyListOf;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyLong;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.anyString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.RETURNS_DEEP_STUBS;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.atLeastOnce;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.inOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.times;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.verifyNoMoreInteractions;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @RunWith(MockitoJUnitRunner.class) public class FetchAndLockRestServiceInteractionTest extends org.camunda.bpm.engine.rest.AbstractRestServiceTest
	public class FetchAndLockRestServiceInteractionTest : AbstractRestServiceTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.rest.util.container.TestContainerRule rule = new org.camunda.bpm.engine.rest.util.container.TestContainerRule();
		public static TestContainerRule rule = new TestContainerRule();

	  private const string FETCH_EXTERNAL_TASK_URL = "/external-task/fetchAndLock";
	  private static readonly string FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE = NamedProcessEngineRestServiceImpl.PATH + "/{name}" + FETCH_EXTERNAL_TASK_URL;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock private org.camunda.bpm.engine.ExternalTaskService externalTaskService;
	  private ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock private org.camunda.bpm.engine.externaltask.ExternalTaskQueryTopicBuilder fetchTopicBuilder;
	  private ExternalTaskQueryTopicBuilder fetchTopicBuilder;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Mock private org.camunda.bpm.engine.IdentityService identityServiceMock;
	  private IdentityService identityServiceMock;

	  private LockedExternalTask lockedExternalTaskMock;

	  private IList<string> groupIds;
	  private IList<string> tenantIds;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUpRuntimeData()
	  public virtual void setUpRuntimeData()
	  {
		when(processEngine.ExternalTaskService).thenReturn(externalTaskService);

		lockedExternalTaskMock = MockProvider.createMockLockedExternalTask();
		when(externalTaskService.fetchAndLock(anyInt(), any(typeof(string)), any(typeof(Boolean)))).thenReturn(fetchTopicBuilder);

		when(fetchTopicBuilder.topic(any(typeof(string)), anyLong())).thenReturn(fetchTopicBuilder);

		when(fetchTopicBuilder.variables(anyListOf(typeof(string)))).thenReturn(fetchTopicBuilder);

		when(fetchTopicBuilder.enableCustomObjectDeserialization()).thenReturn(fetchTopicBuilder);

		// for authentication
		when(processEngine.IdentityService).thenReturn(identityServiceMock);

		IList<Group> groupMocks = MockProvider.createMockGroups();
		groupIds = setupGroupQueryMock(groupMocks);

		IList<Tenant> tenantMocks = Collections.singletonList(MockProvider.createMockTenant());
		tenantIds = setupTenantQueryMock(tenantMocks);

		(new FetchAndLockContextListener()).contextInitialized(mock(typeof(ServletContextEvent), RETURNS_DEEP_STUBS));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchAndLock()
	  public virtual void shouldFetchAndLock()
	  {
		when(fetchTopicBuilder.execute()).thenReturn(new List<LockedExternalTask>(Collections.singleton(lockedExternalTaskMock)));
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(null, true, true, false);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").then().expect().statusCode(Status.OK.StatusCode).body("[0].id", equalTo(MockProvider.EXTERNAL_TASK_ID)).body("[0].topicName", equalTo(MockProvider.EXTERNAL_TASK_TOPIC_NAME)).body("[0].workerId", equalTo(MockProvider.EXTERNAL_TASK_WORKER_ID)).body("[0].lockExpirationTime", equalTo(MockProvider.EXTERNAL_TASK_LOCK_EXPIRATION_TIME)).body("[0].processInstanceId", equalTo(MockProvider.EXAMPLE_PROCESS_INSTANCE_ID)).body("[0].executionId", equalTo(MockProvider.EXAMPLE_EXECUTION_ID)).body("[0].activityId", equalTo(MockProvider.EXAMPLE_ACTIVITY_ID)).body("[0].activityInstanceId", equalTo(MockProvider.EXAMPLE_ACTIVITY_INSTANCE_ID)).body("[0].processDefinitionId", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_ID)).body("[0].processDefinitionKey", equalTo(MockProvider.EXAMPLE_PROCESS_DEFINITION_KEY)).body("[0].tenantId", equalTo(MockProvider.EXAMPLE_TENANT_ID)).body("[0].retries", equalTo(MockProvider.EXTERNAL_TASK_RETRIES)).body("[0].errorMessage", equalTo(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE)).body("[0].errorMessage", equalTo(MockProvider.EXTERNAL_TASK_ERROR_MESSAGE)).body("[0].priority", equalTo(MockProvider.EXTERNAL_TASK_PRIORITY)).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME, notNullValue()).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".value", equalTo(MockProvider.EXAMPLE_PRIMITIVE_VARIABLE_VALUE.Value)).body("[0].variables." + MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME + ".type", equalTo("String")).when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", true);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).variables(Collections.singletonList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchWithoutVariables()
	  public virtual void shouldFetchWithoutVariables()
	  {
		when(fetchTopicBuilder.execute()).thenReturn(new List<LockedExternalTask>(Collections.singleton(lockedExternalTaskMock)));
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(null);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).then().expect().statusCode(Status.OK.StatusCode).body("[0].id", equalTo(MockProvider.EXTERNAL_TASK_ID)).when().post(FETCH_EXTERNAL_TASK_URL);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", false);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFetchWithCustomObjectDeserializationEnabled()
	  public virtual void shouldFetchWithCustomObjectDeserializationEnabled()
	  {
		when(fetchTopicBuilder.execute()).thenReturn(new List<LockedExternalTask>(Collections.singleton(lockedExternalTaskMock)));
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(null, false, true, true);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").then().expect().statusCode(Status.OK.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);

		InOrder inOrder = inOrder(fetchTopicBuilder, externalTaskService);
		inOrder.verify(externalTaskService).fetchAndLock(5, "aWorkerId", false);
		inOrder.verify(fetchTopicBuilder).topic("aTopicName", 12354L);
		inOrder.verify(fetchTopicBuilder).variables(Collections.singletonList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME));
		inOrder.verify(fetchTopicBuilder).enableCustomObjectDeserialization();
		inOrder.verify(fetchTopicBuilder).execute();
		verifyNoMoreInteractions(fetchTopicBuilder, externalTaskService);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowInvalidRequestExceptionOnMaxTimeoutExceeded()
	  public virtual void shouldThrowInvalidRequestExceptionOnMaxTimeoutExceeded()
	  {
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(FetchAndLockHandlerImpl.MAX_REQUEST_TIMEOUT + 1);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").then().expect().body("type", equalTo(typeof(InvalidRequestException).Name)).body("message", containsString("The asynchronous response timeout cannot be set to a value greater than ")).statusCode(Status.BAD_REQUEST.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowProcessEngineExceptionDuringTimeout()
	  public virtual void shouldThrowProcessEngineExceptionDuringTimeout()
	  {
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(500L);

		when(fetchTopicBuilder.execute()).thenReturn(System.Linq.Enumerable.Empty<LockedExternalTask>()).thenReturn(System.Linq.Enumerable.Empty<LockedExternalTask>()).thenThrow(new ProcessEngineException("anExceptionMessage"));

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").then().expect().body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("anExceptionMessage")).statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);

		verify(fetchTopicBuilder, atLeastOnce()).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowProcessEngineExceptionNotDuringTimeout()
	  public virtual void shouldThrowProcessEngineExceptionNotDuringTimeout()
	  {
		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(500L);

		when(fetchTopicBuilder.execute()).thenThrow(new ProcessEngineException("anExceptionMessage"));

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").then().expect().body("type", equalTo(typeof(ProcessEngineException).Name)).body("message", equalTo("anExceptionMessage")).statusCode(Status.INTERNAL_SERVER_ERROR.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);

		verify(fetchTopicBuilder, times(1)).execute();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldResponseImmediatelyDueToAvailableTasks()
	  public virtual void shouldResponseImmediatelyDueToAvailableTasks()
	  {
		when(fetchTopicBuilder.execute()).thenReturn(new List<LockedExternalTask>(Collections.singleton(lockedExternalTaskMock)));

		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(500L);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).then().expect().body("size()", @is(1)).statusCode(Status.OK.StatusCode).when().post(FETCH_EXTERNAL_TASK_URL);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test public void shouldSetAuthenticationProperly()
	  public virtual void shouldSetAuthenticationProperly()
	  {
		when(identityServiceMock.CurrentAuthentication).thenReturn(new Authentication(MockProvider.EXAMPLE_USER_ID, groupIds, tenantIds));

		FetchExternalTasksExtendedDto fetchExternalTasksDto = createDto(500L);

		given().contentType(ContentType.JSON).body(fetchExternalTasksDto).pathParam("name", "default").when().post(FETCH_EXTERNAL_TASK_URL_NAMED_ENGINE);

		ArgumentCaptor<Authentication> argumentCaptor = ArgumentCaptor.forClass(typeof(Authentication));
		verify(identityServiceMock, atLeastOnce()).Authentication = argumentCaptor.capture();

		assertThat(argumentCaptor.Value.UserId, @is(MockProvider.EXAMPLE_USER_ID));
		assertThat(argumentCaptor.Value.GroupIds, @is(groupIds));
		assertThat(argumentCaptor.Value.TenantIds, @is(tenantIds));
	  }

	  // helper /////////////////////////

	  private FetchExternalTasksExtendedDto createDto(long? responseTimeout)
	  {
		return createDto(responseTimeout, false, false, false);
	  }

	  private FetchExternalTasksExtendedDto createDto(long? responseTimeout, bool usePriority, bool withVariables, bool withDeserialization)
	  {
		FetchExternalTasksExtendedDto fetchExternalTasksDto = new FetchExternalTasksExtendedDto();
		if (responseTimeout != null)
		{
		  fetchExternalTasksDto.AsyncResponseTimeout = responseTimeout;
		}
		fetchExternalTasksDto.MaxTasks = 5;
		fetchExternalTasksDto.WorkerId = "aWorkerId";
		fetchExternalTasksDto.UsePriority = usePriority;
		FetchExternalTasksExtendedDto.FetchExternalTaskTopicDto topicDto = new FetchExternalTasksExtendedDto.FetchExternalTaskTopicDto();
		fetchExternalTasksDto.Topics = Collections.singletonList(topicDto);
		topicDto.TopicName = "aTopicName";
		topicDto.LockDuration = 12354L;
		if (withVariables)
		{
		  topicDto.Variables = Collections.singletonList(MockProvider.EXAMPLE_VARIABLE_INSTANCE_NAME);
		}
		topicDto.DeserializeValues = withDeserialization;
		fetchExternalTasksDto.Topics = Collections.singletonList(topicDto);
		return fetchExternalTasksDto;
	  }

	  private IList<string> setupGroupQueryMock(IList<Group> groups)
	  {
		GroupQuery mockGroupQuery = mock(typeof(GroupQuery));

		when(identityServiceMock.createGroupQuery()).thenReturn(mockGroupQuery);
		when(mockGroupQuery.groupMember(anyString())).thenReturn(mockGroupQuery);
		when(mockGroupQuery.list()).thenReturn(groups);

		IList<string> groupIds = new List<string>();
		foreach (Group groupMock in groups)
		{
		  groupIds.Add(groupMock.Id);
		}
		return groupIds;
	  }

	  private IList<string> setupTenantQueryMock(IList<Tenant> tenants)
	  {
		TenantQuery mockTenantQuery = mock(typeof(TenantQuery));

		when(identityServiceMock.createTenantQuery()).thenReturn(mockTenantQuery);
		when(mockTenantQuery.userMember(anyString())).thenReturn(mockTenantQuery);
		when(mockTenantQuery.includingGroupsOfUser(anyBoolean())).thenReturn(mockTenantQuery);
		when(mockTenantQuery.list()).thenReturn(tenants);

		IList<string> tenantIds = new List<string>();
		foreach (Tenant tenant in tenants)
		{
		  tenantIds.Add(tenant.Id);
		}
		return tenantIds;
	  }

	}

}