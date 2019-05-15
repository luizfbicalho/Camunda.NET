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
namespace org.camunda.bpm.engine.test.api.identity
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using Triple = org.apache.commons.lang3.tuple.Triple;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using Group = org.camunda.bpm.engine.identity.Group;
	using Tenant = org.camunda.bpm.engine.identity.Tenant;
	using User = org.camunda.bpm.engine.identity.User;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class IdentityServiceUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public IdentityServiceUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TEST_USER_ID = "newTestUser";
	  protected internal const string TEST_GROUP_ID = "newTestGroup";
	  protected internal const string TEST_TENANT_ID = "newTestTenant";

	  protected internal ProcessEngineRule engineRule = new ProcessEngineRule(true);
	  protected internal ProcessEngineTestRule testRule;

	  protected internal RepositoryService repositoryService;
	  protected internal IdentityService identityService;
	  protected internal HistoryService historyService;
	  protected internal ProcessEngineConfiguration processEngineConfiguration;

	  protected internal UserOperationLogQuery query;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;
		repositoryService = engineRule.RepositoryService;
		identityService = engineRule.IdentityService;
		historyService = engineRule.HistoryService;
		query = historyService.createUserOperationLogQuery();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanUp()
	  public virtual void cleanUp()
	  {
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (Tenant tenant in identityService.createTenantQuery().list())
		{
		  identityService.deleteTenant(tenant.Id);
		}
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogUserCreation()
	  public virtual void shouldLogUserCreation()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.USER, null, TEST_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogUserCreationFailure()
	  public virtual void shouldNotLogUserCreationFailure()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogUserUpdate()
	  public virtual void shouldLogUserUpdate()
	  {
		// given
		User newUser = identityService.newUser(TEST_USER_ID);
		identityService.saveUser(newUser);
		assertEquals(0, query.count());

		// when
		newUser.Email = "test@mail.com";
		identityService.AuthenticatedUserId = "userId";
		identityService.saveUser(newUser);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, EntityTypes.USER, null, TEST_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogUserDeletion()
	  public virtual void shouldLogUserDeletion()
	  {
		// given
		User newUser = identityService.newUser(TEST_USER_ID);
		identityService.saveUser(newUser);
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteUser(newUser.Id);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.USER, null, TEST_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogUserDeletionOnNonExisting()
	  public virtual void shouldNotLogUserDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteUser(TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogUserUnlock()
	  public virtual void shouldLogUserUnlock()
	  {
		// given
		User newUser = identityService.newUser(TEST_USER_ID);
		newUser.Password = "right";
		identityService.saveUser(newUser);
		identityService.checkPassword(TEST_USER_ID, "wrong!");
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.unlockUser(TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UNLOCK, EntityTypes.USER, null, TEST_USER_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogUserUnlockOnNonExistingUser()
	  public virtual void shouldNotLogUserUnlockOnNonExistingUser()
	  {
		// given
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.unlockUser(TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogUserUnlockOnNonExistingLock()
	  public virtual void shouldNotLogUserUnlockOnNonExistingLock()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.unlockUser(TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogGroupCreation()
	  public virtual void shouldLogGroupCreation()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.GROUP, null, TEST_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogGroupCreationFailure()
	  public virtual void shouldNotLogGroupCreationFailure()
	  {
		// given
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogGroupUpdate()
	  public virtual void shouldLogGroupUpdate()
	  {
		// given
		Group newGroup = identityService.newGroup(TEST_GROUP_ID);
		identityService.saveGroup(newGroup);
		assertEquals(0, query.count());

		// when
		newGroup.Name = "testName";
		identityService.AuthenticatedUserId = "userId";
		identityService.saveGroup(newGroup);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, EntityTypes.GROUP, null, TEST_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogGroupDeletion()
	  public virtual void shouldLogGroupDeletion()
	  {
		// given
		Group newGroup = identityService.newGroup(TEST_GROUP_ID);
		identityService.saveGroup(newGroup);
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteGroup(newGroup.Id);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.GROUP, null, TEST_GROUP_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogGroupDeletionOnNonExisting()
	  public virtual void shouldNotLogGroupDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteGroup(TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantCreation()
	  public virtual void shouldLogTenantCreation()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.TENANT, null, TEST_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantCreationFailure()
	  public virtual void shouldNotLogTenantCreationFailure()
	  {
		// given
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantUpdate()
	  public virtual void shouldLogTenantUpdate()
	  {
		// given
		Tenant newTenant = identityService.newTenant(TEST_TENANT_ID);
		identityService.saveTenant(newTenant);
		assertEquals(0, query.count());

		// when
		newTenant.Name = "testName";
		identityService.AuthenticatedUserId = "userId";
		identityService.saveTenant(newTenant);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE, EntityTypes.TENANT, null, TEST_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantDeletion()
	  public virtual void shouldLogTenantDeletion()
	  {
		// given
		Tenant newTenant = identityService.newTenant(TEST_TENANT_ID);
		identityService.saveTenant(newTenant);
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteTenant(newTenant.Id);
		identityService.clearAuthentication();

		// then
		assertLog(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.TENANT, null, TEST_TENANT_ID);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantDeletionOnNonExisting()
	  public virtual void shouldNotLogTenantDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());

		// when
		identityService.AuthenticatedUserId = "userId";
		identityService.deleteTenant(TEST_TENANT_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogGroupMembershipCreation()
	  public virtual void shouldLogGroupMembershipCreation()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.createMembership(TEST_USER_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.GROUP_MEMBERSHIP, Triple.of("userId", (string) null, TEST_USER_ID), Triple.of("groupId", (string) null, TEST_GROUP_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogGroupMembershipCreationFailure()
	  public virtual void shouldNotLogGroupMembershipCreationFailure()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.createMembership(TEST_USER_ID, TEST_GROUP_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.createMembership(TEST_USER_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogGroupMembershipDeletion()
	  public virtual void shouldLogGroupMembershipDeletion()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.createMembership(TEST_USER_ID, TEST_GROUP_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteMembership(TEST_USER_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.GROUP_MEMBERSHIP, Triple.of("userId", (string) null, TEST_USER_ID), Triple.of("groupId", (string) null, TEST_GROUP_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogGroupMembershipDeletionOnNonExisting()
	  public virtual void shouldNotLogGroupMembershipDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteMembership(TEST_USER_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantUserMembershipCreation()
	  public virtual void shouldLogTenantUserMembershipCreation()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.createTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.TENANT_MEMBERSHIP, Triple.of("userId", (string) null, TEST_USER_ID), Triple.of("tenantId", (string) null, TEST_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantUserMembershipCreationFailure()
	  public virtual void shouldNotLogTenantUserMembershipCreationFailure()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.createTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.createTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantUserMembershipDeletion()
	  public virtual void shouldLogTenantUserMembershipDeletion()
	  {
		// given
		identityService.saveUser(identityService.newUser(TEST_USER_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.createTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.TENANT_MEMBERSHIP, Triple.of("userId", (string) null, TEST_USER_ID), Triple.of("tenantId", (string) null, TEST_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantUserMembershipDeletionOnNonExisting()
	  public virtual void shouldNotLogTenantUserMembershipDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteTenantUserMembership(TEST_TENANT_ID, TEST_USER_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantGroupMembershipCreation()
	  public virtual void shouldLogTenantGroupMembershipCreation()
	  {
		// given
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.createTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE, EntityTypes.TENANT_MEMBERSHIP, Triple.of("groupId", (string) null, TEST_GROUP_ID), Triple.of("tenantId", (string) null, TEST_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantGroupMembershipCreationFailure()
	  public virtual void shouldNotLogTenantGroupMembershipCreationFailure()
	  {
		// given
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.createTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		thrown.expect(typeof(ProcessEngineException));
		identityService.createTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldLogTenantGroupMembershipDeletion()
	  public virtual void shouldLogTenantGroupMembershipDeletion()
	  {
		// given
		identityService.saveGroup(identityService.newGroup(TEST_GROUP_ID));
		identityService.saveTenant(identityService.newTenant(TEST_TENANT_ID));
		identityService.createTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertLogs(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, EntityTypes.TENANT_MEMBERSHIP, Triple.of("groupId", (string) null, TEST_GROUP_ID), Triple.of("tenantId", (string) null, TEST_TENANT_ID));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotLogTenantGroupMembershipDeletionOnNonExisting()
	  public virtual void shouldNotLogTenantGroupMembershipDeletionOnNonExisting()
	  {
		// given
		assertEquals(0, query.count());
		identityService.AuthenticatedUserId = "userId";

		// when
		identityService.deleteTenantGroupMembership(TEST_TENANT_ID, TEST_GROUP_ID);
		identityService.clearAuthentication();

		// then
		assertEquals(0, query.count());
	  }

	  protected internal virtual void assertLog(string operation, string entity, string orgValue, string newValue)
	  {
		assertEquals(1, query.count());
		UserOperationLogEntry entry = query.singleResult();
		assertEquals(operation, entry.OperationType);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		assertEquals(entity, entry.EntityType);
		assertEquals(orgValue, entry.OrgValue);
		assertEquals(newValue, entry.NewValue);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SafeVarargs protected final void assertLogs(String operation, String entity, org.apache.commons.lang3.tuple.Triple<String, String, String>... values)
	  protected internal void assertLogs(string operation, string entity, params Triple<string, string, string>[] values)
	  {
		assertEquals(values.Length, query.count());
		foreach (Triple<string, string, string> valueTriple in values)
		{
		  UserOperationLogEntry entry = query.property(valueTriple.Left).singleResult();
		  assertEquals(operation, entry.OperationType);
		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, entry.Category);
		  assertEquals(entity, entry.EntityType);
		  assertEquals(valueTriple.Middle, entry.OrgValue);
		  assertEquals(valueTriple.Right, entry.NewValue);
		}
	  }
	}

}