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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.containsInAnyOrder;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.*;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using TaskQueryImpl = org.camunda.bpm.engine.impl.TaskQueryImpl;
	using AuthorizationCheck = org.camunda.bpm.engine.impl.db.AuthorizationCheck;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using Session = org.camunda.bpm.engine.impl.interceptor.Session;
	using AuthorizationManager = org.camunda.bpm.engine.impl.persistence.entity.AuthorizationManager;
	using ArgumentCaptor = org.mockito.ArgumentCaptor;

	public class GroupAuthorizationTest : AuthorizationTest
	{

	  public const string testUserId = "testUser";
	  public static readonly IList<string> testGroupIds = Arrays.asList("testGroup1", "testGroup2", "testGroup3");

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		createUser(testUserId);
		foreach (string testGroupId in testGroupIds)
		{
		  createGroupAndAddUser(testGroupId, testUserId);
		}

		identityService.setAuthentication(testUserId, testGroupIds);
		processEngineConfiguration.AuthorizationEnabled = true;
	  }


	  public virtual void testTaskQueryWithoutGroupAuthorizations()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(outerInstance.processEngine.TaskService.createTaskQuery());
			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
			when(taskQuery.AuthCheck).thenReturn(authCheck);

			taskQuery.list();

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));
			verify(authCheck).AuthGroupIds = eq(System.Linq.Enumerable.Empty<string>());

			return null;
		  }
	  }

	  public virtual void testTaskQueryWithOneGroupAuthorization()
	  {
		createGroupGrantAuthorization(Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, testGroupIds[0]);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass2(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(outerInstance.processEngine.TaskService.createTaskQuery());
			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
			when(taskQuery.AuthCheck).thenReturn(authCheck);

			taskQuery.list();

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));
			verify(authCheck).AuthGroupIds = eq(testGroupIds.subList(0, 1));

			return null;
		  }
	  }

	  public virtual void testTaskQueryWithGroupAuthorization()
	  {
		foreach (string testGroupId in testGroupIds)
		{
		  createGroupGrantAuthorization(Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, testGroupId);
		}

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass3(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(outerInstance.processEngine.TaskService.createTaskQuery());
			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
			when(taskQuery.AuthCheck).thenReturn(authCheck);

			taskQuery.list();

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));
			verify(authCheck, atLeastOnce()).AuthGroupIds = (IList<string>) argThat(containsInAnyOrder(testGroupIds.ToArray()));

			return null;
		  }
	  }

	  public virtual void testTaskQueryWithUserWithoutGroups()
	  {
		identityService.setAuthentication(testUserId, null);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass4(this));
	  }

	  private class CommandAnonymousInnerClass4 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass4(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));

			TaskQueryImpl taskQuery = (TaskQueryImpl) spy(outerInstance.processEngine.TaskService.createTaskQuery());
			AuthorizationCheck authCheck = spy(new AuthorizationCheck());
			when(taskQuery.AuthCheck).thenReturn(authCheck);

			taskQuery.list();

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq((IList<string>) null));
			verify(authCheck).AuthGroupIds = eq(System.Linq.Enumerable.Empty<string>());

			return null;
		  }
	  }

	  public virtual void testCheckAuthorizationWithoutGroupAuthorizations()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass5(this));
	  }

	  private class CommandAnonymousInnerClass5 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass5(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

			outerInstance.authorizationService.isUserAuthorized(testUserId, testGroupIds, Permissions.READ, Resources.TASK);

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));

			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.forClass(typeof(AuthorizationCheck));
			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
			assertTrue(authorizationCheck.AuthGroupIds.Count == 0);

			return null;
		  }
	  }

	  public virtual void testCheckAuthorizationWithOneGroupAuthorizations()
	  {
		createGroupGrantAuthorization(Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, testGroupIds[0]);

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass6(this));
	  }

	  private class CommandAnonymousInnerClass6 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass6(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

			outerInstance.authorizationService.isUserAuthorized(testUserId, testGroupIds, Permissions.READ, Resources.TASK);

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));

			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.forClass(typeof(AuthorizationCheck));
			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
			assertEquals(testGroupIds.subList(0, 1), authorizationCheck.AuthGroupIds);

			return null;
		  }
	  }

	  public virtual void testCheckAuthorizationWithGroupAuthorizations()
	  {
		foreach (string testGroupId in testGroupIds)
		{
		  createGroupGrantAuthorization(Resources.TASK, org.camunda.bpm.engine.authorization.Authorization_Fields.ANY, testGroupId);
		}

		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass7(this));
	  }

	  private class CommandAnonymousInnerClass7 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass7(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

			outerInstance.authorizationService.isUserAuthorized(testUserId, testGroupIds, Permissions.READ, Resources.TASK);

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq(testGroupIds));

			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.forClass(typeof(AuthorizationCheck));
			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
			assertThat(authorizationCheck.AuthGroupIds, containsInAnyOrder(testGroupIds.ToArray()));

			return null;
		  }
	  }

	  public virtual void testCheckAuthorizationWithUserWithoutGroups()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass8(this));
	  }

	  private class CommandAnonymousInnerClass8 : Command<Void>
	  {
		  private readonly GroupAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass8(GroupAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			AuthorizationManager authorizationManager = outerInstance.spyOnSession(commandContext, typeof(AuthorizationManager));
			DbEntityManager dbEntityManager = outerInstance.spyOnSession(commandContext, typeof(DbEntityManager));

			outerInstance.authorizationService.isUserAuthorized(testUserId, null, Permissions.READ, Resources.TASK);

			verify(authorizationManager, atLeastOnce()).filterAuthenticatedGroupIds(eq((IList<string>) null));

			ArgumentCaptor<AuthorizationCheck> authorizationCheckArgument = ArgumentCaptor.forClass(typeof(AuthorizationCheck));
			verify(dbEntityManager).selectBoolean(eq("isUserAuthorizedForResource"), authorizationCheckArgument.capture());

			AuthorizationCheck authorizationCheck = authorizationCheckArgument.Value;
			assertTrue(authorizationCheck.AuthGroupIds.Count == 0);

			return null;
		  }
	  }

	  protected internal virtual void createGroupGrantAuthorization(Resource resource, string resourceId, string groupId, params Permission[] permissions)
	  {
		Authorization authorization = createGrantAuthorization(resource, resourceId);
		authorization.GroupId = groupId;
		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}
		saveAuthorization(authorization);
	  }

	  protected internal virtual void createGroupAndAddUser(string groupId, string userId)
	  {
		createGroup(groupId);
		identityService.createMembership(userId, groupId);
	  }

	  protected internal virtual T spyOnSession<T>(CommandContext commandContext, Type<T> sessionClass) where T : org.camunda.bpm.engine.impl.interceptor.Session
	  {
		T manager = commandContext.getSession(sessionClass);
		T spy = spy(manager);
		commandContext.Sessions[sessionClass] = spy;

		return spy;
	  }

	}

}