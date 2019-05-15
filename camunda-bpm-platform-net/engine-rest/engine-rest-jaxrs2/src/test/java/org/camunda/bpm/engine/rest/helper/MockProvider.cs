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
namespace org.camunda.bpm.engine.rest.helper
{
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using DateTimeUtil = org.camunda.bpm.engine.impl.calendar.DateTimeUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using StringValue = org.camunda.bpm.engine.variable.value.StringValue;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.util.DateTimeUtils.withTimezone;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	/// <summary>
	/// Provides mocks for the basic engine entities, such as
	/// <seealso cref="ProcessDefinition"/>, <seealso cref="User"/>, etc., that are reused across the
	/// various kinds of tests.
	/// </summary>
	public abstract class MockProvider
	{

	  public const string EXAMPLE_USER_ID = "userId";
	  public const string EXAMPLE_USER_PASSWORD = "s3cret";
	  public const string EXAMPLE_VARIABLE_INSTANCE_NAME = "aVariableInstanceName";
	  public const string EXTERNAL_TASK_ID = "anExternalTaskId";
	  public const string EXTERNAL_TASK_TOPIC_NAME = "aTopic";
	  public const string EXTERNAL_TASK_WORKER_ID = "aWorkerId";
	  public static readonly string EXTERNAL_TASK_LOCK_EXPIRATION_TIME = withTimezone("2015-10-05T13:25:00");
	  public const string EXAMPLE_PROCESS_INSTANCE_ID = "aProcInstId";
	  public const string EXAMPLE_EXECUTION_ID = "anExecutionId";
	  public const string EXAMPLE_ACTIVITY_ID = "anActivity";
	  public const string EXAMPLE_ACTIVITY_INSTANCE_ID = "anActivityInstanceId";
	  public const string EXAMPLE_PROCESS_DEFINITION_ID = "aProcDefId";
	  public const string EXAMPLE_PROCESS_DEFINITION_KEY = "aKey";
	  public const string EXAMPLE_TENANT_ID = "aTenantId";
	  public static readonly int? EXTERNAL_TASK_RETRIES = new int?(5);
	  public const string EXTERNAL_TASK_ERROR_MESSAGE = "some error";
	  public static readonly long EXTERNAL_TASK_PRIORITY = int.MaxValue + 466L;
	  public static readonly StringValue EXAMPLE_PRIMITIVE_VARIABLE_VALUE = Variables.stringValue("aVariableInstanceValue");
	  public const bool EXTERNAL_TASK_SUSPENDED = true;
	  public const string EXAMPLE_GROUP_ID = "groupId1";
	  public const string EXAMPLE_GROUP_NAME = "group1";
	  public const string EXAMPLE_GROUP_TYPE = "organizational-unit";
	  public const string EXAMPLE_GROUP_NAME_UPDATE = "group1Update";
	  public const string EXAMPLE_USER_FIRST_NAME = "firstName";
	  public const string EXAMPLE_USER_LAST_NAME = "lastName";
	  public const string EXAMPLE_USER_EMAIL = "test@example.org";
	  public const string EXAMPLE_TENANT_NAME = "aTenantName";
	  // engine
	  public const string EXAMPLE_PROCESS_ENGINE_NAME = "default";
	  public const string ANOTHER_EXAMPLE_PROCESS_ENGINE_NAME = "anotherEngineName";
	  public const string NON_EXISTING_PROCESS_ENGINE_NAME = "aNonExistingEngineName";

	  public static LockedExternalTask createMockLockedExternalTask()
	  {
		return mockExternalTask().variable(EXAMPLE_VARIABLE_INSTANCE_NAME, EXAMPLE_PRIMITIVE_VARIABLE_VALUE).buildLockedExternalTask();
	  }

	  public static MockExternalTaskBuilder mockExternalTask()
	  {
		return (new MockExternalTaskBuilder()).id(EXTERNAL_TASK_ID).activityId(EXAMPLE_ACTIVITY_ID).activityInstanceId(EXAMPLE_ACTIVITY_INSTANCE_ID).errorMessage(EXTERNAL_TASK_ERROR_MESSAGE).executionId(EXAMPLE_EXECUTION_ID).lockExpirationTime(DateTimeUtil.parseDate(EXTERNAL_TASK_LOCK_EXPIRATION_TIME)).processDefinitionId(EXAMPLE_PROCESS_DEFINITION_ID).processDefinitionKey(EXAMPLE_PROCESS_DEFINITION_KEY).processInstanceId(EXAMPLE_PROCESS_INSTANCE_ID).retries(EXTERNAL_TASK_RETRIES).suspended(EXTERNAL_TASK_SUSPENDED).topicName(EXTERNAL_TASK_TOPIC_NAME).workerId(EXTERNAL_TASK_WORKER_ID).tenantId(EXAMPLE_TENANT_ID).priority(EXTERNAL_TASK_PRIORITY);

	  }


	  // user, groups and tenants

	  public static Group createMockGroup()
	  {
		return mockGroup().build();
	  }

	  public static MockGroupBuilder mockGroup()
	  {
		return (new MockGroupBuilder()).id(EXAMPLE_GROUP_ID).name(EXAMPLE_GROUP_NAME).type(EXAMPLE_GROUP_TYPE);
	  }

	  public static Group createMockGroupUpdate()
	  {
		Group mockGroup = mock(typeof(Group));
		when(mockGroup.Id).thenReturn(EXAMPLE_GROUP_ID);
		when(mockGroup.Name).thenReturn(EXAMPLE_GROUP_NAME_UPDATE);

		return mockGroup;
	  }

	  public static IList<Group> createMockGroups()
	  {
		IList<Group> mockGroups = new List<Group>();
		mockGroups.Add(createMockGroup());
		return mockGroups;
	  }

	  public static User createMockUser()
	  {
		return mockUser().build();
	  }

	  public static MockUserBuilder mockUser()
	  {
		return (new MockUserBuilder()).id(EXAMPLE_USER_ID).firstName(EXAMPLE_USER_FIRST_NAME).lastName(EXAMPLE_USER_LAST_NAME).email(EXAMPLE_USER_EMAIL).password(EXAMPLE_USER_PASSWORD);
	  }

	  public static Tenant createMockTenant()
	  {
		Tenant mockTenant = mock(typeof(Tenant));
		when(mockTenant.Id).thenReturn(EXAMPLE_TENANT_ID);
		when(mockTenant.Name).thenReturn(EXAMPLE_TENANT_NAME);
		return mockTenant;
	  }

	}

}