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
namespace org.camunda.bpm.engine.test.api.authorization.history
{

	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using DefaultPermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.DefaultPermissionProvider;
	using PermissionProvider = org.camunda.bpm.engine.impl.cfg.auth.PermissionProvider;
	using StringUtil = org.camunda.bpm.engine.impl.util.StringUtil;
	using TestPermissions = org.camunda.bpm.engine.test.api.identity.TestPermissions;
	using TestResource = org.camunda.bpm.engine.test.api.identity.TestResource;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class AuthorizationUserOperationLogTest : AuthorizationTest
	{

	  public virtual void testLogCreatedOnAuthorizationCreation()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(0, query.count());

		// when
		createGrantAuthorizationGroup(Resources.PROCESS_DEFINITION, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testGroupId", ProcessDefinitionPermissions.DELETE);

		// then
		assertEquals(6, query.count());

		UserOperationLogEntry entry = query.property("permissionBits").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(ProcessDefinitionPermissions.DELETE.Value.ToString(), entry.NewValue);

		entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(ProcessDefinitionPermissions.DELETE.Name, entry.NewValue);

		entry = query.property("type").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT.ToString(), entry.NewValue);

		entry = query.property("resource").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(Resources.PROCESS_DEFINITION.resourceName(), entry.NewValue);

		entry = query.property("resourceId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, entry.NewValue);

		entry = query.property("groupId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals("testGroupId", entry.NewValue);
	  }

	  public virtual void testLogCreatedOnAuthorizationUpdate()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		identityService.clearAuthentication();
		Authorization authorization = createGrantAuthorization(Resources.PROCESS_DEFINITION, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testUserId", Permissions.DELETE);
		assertEquals(0, query.count());
		identityService.setAuthentication(userId, Arrays.asList(groupId));

		// when
		authorization.addPermission(Permissions.READ);
		authorization.Resource = Resources.PROCESS_INSTANCE;
		authorization.ResourceId = "abc123";
		authorization.GroupId = "testGroupId";
		authorization.UserId = null;
		saveAuthorization(authorization);

		// then
		assertEquals(7, query.count());

		UserOperationLogEntry entry = query.property("permissionBits").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals((Permissions.DELETE.Value | Permissions.READ.Value).ToString(), entry.NewValue);
		assertEquals(Permissions.DELETE.Value.ToString(), entry.OrgValue);

		entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(Permissions.READ.Name + ", " + Permissions.DELETE.Name, entry.NewValue);
		assertEquals(Permissions.DELETE.Name, entry.OrgValue);

		entry = query.property("type").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT.ToString(), entry.NewValue);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT.ToString(), entry.OrgValue);

		entry = query.property("resource").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(Resources.PROCESS_INSTANCE.resourceName(), entry.NewValue);
		assertEquals(Resources.PROCESS_DEFINITION.resourceName(), entry.OrgValue);

		entry = query.property("resourceId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals("abc123", entry.NewValue);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, entry.OrgValue);

		entry = query.property("userId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertNull(entry.NewValue);
		assertEquals("testUserId", entry.OrgValue);

		entry = query.property("groupId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals("testGroupId", entry.NewValue);
		assertNull(entry.OrgValue);
	  }

	  public virtual void testLogCreatedOnAuthorizationDeletion()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		identityService.clearAuthentication();
		Authorization authorization = createGrantAuthorization(Resources.PROCESS_DEFINITION, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testUserId", ProcessDefinitionPermissions.DELETE);
		assertEquals(0, query.count());
		identityService.setAuthentication(userId, Arrays.asList(groupId));

		// when
		authorizationService.deleteAuthorization(authorization.Id);

		// then
		assertEquals(6, query.count());

		UserOperationLogEntry entry = query.property("permissionBits").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(ProcessDefinitionPermissions.DELETE.Value.ToString(), entry.NewValue);

		entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(ProcessDefinitionPermissions.DELETE.Name, entry.NewValue);

		entry = query.property("type").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT.ToString(), entry.NewValue);

		entry = query.property("resource").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(Resources.PROCESS_DEFINITION.resourceName(), entry.NewValue);

		entry = query.property("resourceId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, entry.NewValue);

		entry = query.property("userId").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals("testUserId", entry.NewValue);
	  }

	  public virtual void testLogCreatedOnAuthorizationCreationWithExceedingPermissionStringList()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(0, query.count());

		// when
		PermissionProvider permissionProvider = processEngineConfiguration.PermissionProvider;
		processEngineConfiguration.PermissionProvider = new TestPermissionProvider();
		createGrantAuthorizationGroup(TestResource.RESOURCE1, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testGroupId", TestPermissions.LONG_NAME);
		processEngineConfiguration.PermissionProvider = permissionProvider;

		// then
		assertEquals(6, query.count());

		UserOperationLogEntry entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(TestPermissions.LONG_NAME.Name.substring(0, StringUtil.DB_MAX_STRING_LENGTH), entry.NewValue);
	  }

	  public virtual void testLogCreatedOnAuthorizationCreationWithAllPermission()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(0, query.count());

		// when
		PermissionProvider permissionProvider = processEngineConfiguration.PermissionProvider;
		processEngineConfiguration.PermissionProvider = new TestPermissionProvider();
		createGrantAuthorizationGroup(TestResource.RESOURCE1, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testGroupId", TestPermissions.ALL);
		processEngineConfiguration.PermissionProvider = permissionProvider;

		// then
		assertEquals(6, query.count());

		UserOperationLogEntry entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(TestPermissions.ALL.Name, entry.NewValue);
	  }

	  public virtual void testLogCreatedOnAuthorizationCreationWithNonePermission()
	  {
		// given
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();
		assertEquals(0, query.count());

		// when
		PermissionProvider permissionProvider = processEngineConfiguration.PermissionProvider;
		processEngineConfiguration.PermissionProvider = new TestPermissionProvider();
		createGrantAuthorizationGroup(TestResource.RESOURCE1, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, "testGroupId", TestPermissions.NONE);
		processEngineConfiguration.PermissionProvider = permissionProvider;

		// then
		assertEquals(6, query.count());

		UserOperationLogEntry entry = query.property("permissions").singleResult();
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(EntityTypes.AUTHORIZATION, entry.EntityType);
		assertEquals(TestPermissions.NONE.Name, entry.NewValue);
	  }

	  public class TestPermissionProvider : DefaultPermissionProvider
	  {
		public override string getNameForResource(int resourceType)
		{
		  foreach (Resource resource in TestResource.values())
		  {
			if (resourceType == resource.resourceType())
			{
			  return resource.resourceName();
			}
		  }
		  return null;
		}

		public override Permission[] getPermissionsForResource(int resourceType)
		{
		  return TestPermissions.values();
		}
	  }
	}

}