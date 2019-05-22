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
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GLOBAL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_GRANT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.AUTH_TYPE_REVOKE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.ALL;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.AUTHORIZATION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.USER;


	using Authorization = org.camunda.bpm.engine.authorization.Authorization;
	using Permission = org.camunda.bpm.engine.authorization.Permission;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resource = org.camunda.bpm.engine.authorization.Resource;
	using Group = org.camunda.bpm.engine.identity.Group;
	using User = org.camunda.bpm.engine.identity.User;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Authentication = org.camunda.bpm.engine.impl.identity.Authentication;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Query = org.camunda.bpm.engine.query.Query;
	using DecisionDefinition = org.camunda.bpm.engine.repository.DecisionDefinition;
	using Deployment = org.camunda.bpm.engine.repository.Deployment;
	using DeploymentBuilder = org.camunda.bpm.engine.repository.DeploymentBuilder;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Variables = org.camunda.bpm.engine.variable.Variables;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public abstract class AuthorizationTest : PluggableProcessEngineTestCase
	{

	  protected internal string userId = "test";
	  protected internal string groupId = "accounting";
	  protected internal User user;
	  protected internal Group group;

	  protected internal const string VARIABLE_NAME = "aVariableName";
	  protected internal const string VARIABLE_VALUE = "aVariableValue";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		user = createUser(userId);
		group = createGroup(groupId);

		identityService.createMembership(userId, groupId);

		identityService.setAuthentication(userId, Arrays.asList(groupId));
		processEngineConfiguration.AuthorizationEnabled = true;
	  }

	  public override void tearDown()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
		foreach (User user in identityService.createUserQuery().list())
		{
		  identityService.deleteUser(user.Id);
		}
		foreach (Group group in identityService.createGroupQuery().list())
		{
		  identityService.deleteGroup(group.Id);
		}
		foreach (Authorization authorization in authorizationService.createAuthorizationQuery().list())
		{
		  authorizationService.deleteAuthorization(authorization.Id);
		}
	  }

	  protected internal virtual T runWithoutAuthorization<T>(Callable<T> runnable)
	  {
		bool authorizationEnabled = processEngineConfiguration.AuthorizationEnabled;
		try
		{
		  disableAuthorization();
		  return runnable.call();
		}
		catch (Exception e)
		{
		  throw e;
		}
		catch (Exception e)
		{
		  throw new Exception(e);
		}
		finally
		{
		  if (authorizationEnabled)
		  {
			enableAuthorization();
		  }
		}
	  }

	  // user ////////////////////////////////////////////////////////////////

	  protected internal virtual User createUser(string userId)
	  {
		User user = identityService.newUser(userId);
		identityService.saveUser(user);

		// give user all permission to manipulate authorizations
		Authorization authorization = createGrantAuthorization(AUTHORIZATION, ANY);
		authorization.UserId = userId;
		authorization.addPermission(ALL);
		saveAuthorization(authorization);

		// give user all permission to manipulate users
		authorization = createGrantAuthorization(USER, ANY);
		authorization.UserId = userId;
		authorization.addPermission(Permissions.ALL);
		saveAuthorization(authorization);

		return user;
	  }

	  // group //////////////////////////////////////////////////////////////

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.identity.Group createGroup(final String groupId)
	  protected internal virtual Group createGroup(string groupId)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass(this, groupId));
	  }

	  private class CallableAnonymousInnerClass : Callable<Group>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string groupId;

		  public CallableAnonymousInnerClass(AuthorizationTest outerInstance, string groupId)
		  {
			  this.outerInstance = outerInstance;
			  this.groupId = groupId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.identity.Group call() throws Exception
		  public Group call()
		  {
			Group group = outerInstance.identityService.newGroup(groupId);
			outerInstance.identityService.saveGroup(group);
			return group;
		  }
	  }

	  // authorization ///////////////////////////////////////////////////////

	  protected internal virtual Authorization createGrantAuthorization(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authorization authorization = createGrantAuthorization(resource, resourceId);
		authorization.UserId = userId;
		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}
		saveAuthorization(authorization);
		return authorization;
	  }

	  protected internal virtual Authorization createGrantAuthorizationWithoutAuthentication(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authentication currentAuthentication = identityService.CurrentAuthentication;
		identityService.clearAuthentication();
		try
		{
		  return createGrantAuthorization(resource, resourceId, userId, permissions);
		}
		finally
		{
		  identityService.Authentication = currentAuthentication;
		}
	  }

	  protected internal virtual void createGrantAuthorizationGroup(Resource resource, string resourceId, string groupId, params Permission[] permissions)
	  {
		Authorization authorization = createGrantAuthorization(resource, resourceId);
		authorization.GroupId = groupId;
		foreach (Permission permission in permissions)
		{
		  authorization.addPermission(permission);
		}
		saveAuthorization(authorization);
	  }

	  protected internal virtual void createRevokeAuthorizationWithoutAuthentication(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authentication currentAuthentication = identityService.CurrentAuthentication;
		identityService.clearAuthentication();
		try
		{
		  createRevokeAuthorization(resource, resourceId, userId, permissions);
		}
		finally
		{
		  identityService.Authentication = currentAuthentication;
		}
	  }

	  protected internal virtual void createRevokeAuthorization(Resource resource, string resourceId, string userId, params Permission[] permissions)
	  {
		Authorization authorization = createRevokeAuthorization(resource, resourceId);
		authorization.UserId = userId;
		foreach (Permission permission in permissions)
		{
		  authorization.removePermission(permission);
		}
		saveAuthorization(authorization);
	  }

	  protected internal virtual Authorization createGlobalAuthorization(Resource resource, string resourceId)
	  {
		Authorization authorization = createAuthorization(AUTH_TYPE_GLOBAL, resource, resourceId);
		return authorization;
	  }

	  protected internal virtual Authorization createGrantAuthorization(Resource resource, string resourceId)
	  {
		Authorization authorization = createAuthorization(AUTH_TYPE_GRANT, resource, resourceId);
		return authorization;
	  }

	  protected internal virtual Authorization createRevokeAuthorization(Resource resource, string resourceId)
	  {
		Authorization authorization = createAuthorization(AUTH_TYPE_REVOKE, resource, resourceId);
		return authorization;
	  }

	  protected internal virtual Authorization createAuthorization(int type, Resource resource, string resourceId)
	  {
		Authorization authorization = authorizationService.createNewAuthorization(type);

		authorization.Resource = resource;
		if (!string.ReferenceEquals(resourceId, null))
		{
		  authorization.ResourceId = resourceId;
		}

		return authorization;
	  }

	  protected internal virtual void saveAuthorization(Authorization authorization)
	  {
		authorizationService.saveAuthorization(authorization);
	  }

	  // enable/disable authorization //////////////////////////////////////////////

	  protected internal virtual void enableAuthorization()
	  {
		processEngineConfiguration.AuthorizationEnabled = true;
	  }

	  protected internal virtual void disableAuthorization()
	  {
		processEngineConfiguration.AuthorizationEnabled = false;
	  }

	  // actions (executed without authorization) ///////////////////////////////////

	  protected internal virtual ProcessInstance startProcessInstanceByKey(string key)
	  {
		return startProcessInstanceByKey(key, null);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.runtime.ProcessInstance startProcessInstanceByKey(final String key, final java.util.Map<String, Object> variables)
	  protected internal virtual ProcessInstance startProcessInstanceByKey(string key, IDictionary<string, object> variables)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass2(this, key, variables));
	  }

	  private class CallableAnonymousInnerClass2 : Callable<ProcessInstance>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string key;
		  private IDictionary<string, object> variables;

		  public CallableAnonymousInnerClass2(AuthorizationTest outerInstance, string key, IDictionary<string, object> variables)
		  {
			  this.outerInstance = outerInstance;
			  this.key = key;
			  this.variables = variables;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ProcessInstance call() throws Exception
		  public ProcessInstance call()
		  {
			return outerInstance.runtimeService.startProcessInstanceByKey(key, variables);
		  }
	  }

	  public override void executeAvailableJobs()
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass3(this));
	  }

	  private class CallableAnonymousInnerClass3 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  public CallableAnonymousInnerClass3(AuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.executeAvailableJobs();
			return null;
		  }
	  }

	  protected internal virtual CaseInstance createCaseInstanceByKey(string key)
	  {
		return createCaseInstanceByKey(key, null);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.runtime.CaseInstance createCaseInstanceByKey(final String key, final java.util.Map<String, Object> variables)
	  protected internal virtual CaseInstance createCaseInstanceByKey(string key, IDictionary<string, object> variables)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass4(this, key, variables));
	  }

	  private class CallableAnonymousInnerClass4 : Callable<CaseInstance>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string key;
		  private IDictionary<string, object> variables;

		  public CallableAnonymousInnerClass4(AuthorizationTest outerInstance, string key, IDictionary<string, object> variables)
		  {
			  this.outerInstance = outerInstance;
			  this.key = key;
			  this.variables = variables;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.CaseInstance call() throws Exception
		  public CaseInstance call()
		  {
			return outerInstance.caseService.createCaseInstanceByKey(key, variables);
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void createTask(final String taskId)
	  protected internal virtual void createTask(string taskId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass5(this, taskId));
	  }

	  private class CallableAnonymousInnerClass5 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;

		  public CallableAnonymousInnerClass5(AuthorizationTest outerInstance, string taskId)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			Task task = outerInstance.taskService.newTask(taskId);
			outerInstance.taskService.saveTask(task);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteTask(final String taskId, final boolean cascade)
	  protected internal virtual void deleteTask(string taskId, bool cascade)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass6(this, taskId, cascade));
	  }

	  private class CallableAnonymousInnerClass6 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private bool cascade;

		  public CallableAnonymousInnerClass6(AuthorizationTest outerInstance, string taskId, bool cascade)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.cascade = cascade;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.deleteTask(taskId, cascade);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void addCandidateUser(final String taskId, final String user)
	  protected internal virtual void addCandidateUser(string taskId, string user)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass7(this, taskId, user));
	  }

	  private class CallableAnonymousInnerClass7 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string user;

		  public CallableAnonymousInnerClass7(AuthorizationTest outerInstance, string taskId, string user)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.user = user;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.addCandidateUser(taskId, user);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void addCandidateGroup(final String taskId, final String group)
	  protected internal virtual void addCandidateGroup(string taskId, string group)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass8(this, taskId, group));
	  }

	  private class CallableAnonymousInnerClass8 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string group;

		  public CallableAnonymousInnerClass8(AuthorizationTest outerInstance, string taskId, string group)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.group = group;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.addCandidateGroup(taskId, group);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setAssignee(final String taskId, final String userId)
	  protected internal virtual void setAssignee(string taskId, string userId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass9(this, taskId, userId));
	  }

	  private class CallableAnonymousInnerClass9 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string userId;

		  public CallableAnonymousInnerClass9(AuthorizationTest outerInstance, string taskId, string userId)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.userId = userId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.setAssignee(taskId, userId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void delegateTask(final String taskId, final String userId)
	  protected internal virtual void delegateTask(string taskId, string userId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass10(this, taskId, userId));
	  }

	  private class CallableAnonymousInnerClass10 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string userId;

		  public CallableAnonymousInnerClass10(AuthorizationTest outerInstance, string taskId, string userId)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.userId = userId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.delegateTask(taskId, userId);
			return null;
		  }
	  }

	  protected internal virtual Task selectSingleTask()
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass11(this));
	  }

	  private class CallableAnonymousInnerClass11 : Callable<Task>
	  {
		  private readonly AuthorizationTest outerInstance;

		  public CallableAnonymousInnerClass11(AuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.task.Task call() throws Exception
		  public Task call()
		  {
			return outerInstance.taskService.createTaskQuery().singleResult();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setTaskVariables(final String taskId, final java.util.Map<String, ? extends Object> variables)
	  protected internal virtual void setTaskVariables<T1>(string taskId, IDictionary<T1> variables) where T1 : object
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass12(this, taskId, variables));
	  }

	  private class CallableAnonymousInnerClass12 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private IDictionary<T1> variables;

		  public CallableAnonymousInnerClass12(AuthorizationTest outerInstance, string taskId, IDictionary<T1> variables)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.variables = variables;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.setVariables(taskId, variables);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setTaskVariablesLocal(final String taskId, final java.util.Map<String, ? extends Object> variables)
	  protected internal virtual void setTaskVariablesLocal<T1>(string taskId, IDictionary<T1> variables) where T1 : object
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass13(this, taskId, variables));
	  }

	  private class CallableAnonymousInnerClass13 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private IDictionary<T1> variables;

		  public CallableAnonymousInnerClass13(AuthorizationTest outerInstance, string taskId, IDictionary<T1> variables)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.variables = variables;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.setVariablesLocal(taskId, variables);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setTaskVariable(final String taskId, final String name, final Object value)
	  protected internal virtual void setTaskVariable(string taskId, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass14(this, taskId, name, value));
	  }

	  private class CallableAnonymousInnerClass14 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass14(AuthorizationTest outerInstance, string taskId, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.setVariable(taskId, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setTaskVariableLocal(final String taskId, final String name, final Object value)
	  protected internal virtual void setTaskVariableLocal(string taskId, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass15(this, taskId, name, value));
	  }

	  private class CallableAnonymousInnerClass15 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string taskId;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass15(AuthorizationTest outerInstance, string taskId, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.taskId = taskId;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.taskService.setVariableLocal(taskId, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setExecutionVariable(final String executionId, final String name, final Object value)
	  protected internal virtual void setExecutionVariable(string executionId, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass16(this, executionId, name, value));
	  }

	  private class CallableAnonymousInnerClass16 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string executionId;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass16(AuthorizationTest outerInstance, string executionId, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.executionId = executionId;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.runtimeService.setVariable(executionId, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setExecutionVariableLocal(final String executionId, final String name, final Object value)
	  protected internal virtual void setExecutionVariableLocal(string executionId, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass17(this, executionId, name, value));
	  }

	  private class CallableAnonymousInnerClass17 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string executionId;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass17(AuthorizationTest outerInstance, string executionId, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.executionId = executionId;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.runtimeService.setVariableLocal(executionId, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setCaseVariable(final String caseExecution, final String name, final Object value)
	  protected internal virtual void setCaseVariable(string caseExecution, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass18(this, caseExecution, name, value));
	  }

	  private class CallableAnonymousInnerClass18 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string caseExecution;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass18(AuthorizationTest outerInstance, string caseExecution, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecution = caseExecution;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.caseService.setVariable(caseExecution, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void setCaseVariableLocal(final String caseExecution, final String name, final Object value)
	  protected internal virtual void setCaseVariableLocal(string caseExecution, string name, object value)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass19(this, caseExecution, name, value));
	  }

	  private class CallableAnonymousInnerClass19 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string caseExecution;
		  private string name;
		  private object value;

		  public CallableAnonymousInnerClass19(AuthorizationTest outerInstance, string caseExecution, string name, object value)
		  {
			  this.outerInstance = outerInstance;
			  this.caseExecution = caseExecution;
			  this.name = name;
			  this.value = value;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.caseService.setVariableLocal(caseExecution, name, value);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.repository.ProcessDefinition selectProcessDefinitionByKey(final String processDefinitionKey)
	  protected internal virtual ProcessDefinition selectProcessDefinitionByKey(string processDefinitionKey)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass20(this, processDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass20 : Callable<ProcessDefinition>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionKey;

		  public CallableAnonymousInnerClass20(AuthorizationTest outerInstance, string processDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionKey = processDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.repository.ProcessDefinition call() throws Exception
		  public ProcessDefinition call()
		  {
			return outerInstance.repositoryService.createProcessDefinitionQuery().processDefinitionKey(processDefinitionKey).singleResult();
		  }
	  }

	  protected internal virtual ProcessInstance selectSingleProcessInstance()
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass21(this));
	  }

	  private class CallableAnonymousInnerClass21 : Callable<ProcessInstance>
	  {
		  private readonly AuthorizationTest outerInstance;

		  public CallableAnonymousInnerClass21(AuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ProcessInstance call() throws Exception
		  public ProcessInstance call()
		  {
			return outerInstance.runtimeService.createProcessInstanceQuery().singleResult();
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendProcessDefinitionByKey(final String processDefinitionKey)
	  protected internal virtual void suspendProcessDefinitionByKey(string processDefinitionKey)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass22(this, processDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass22 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionKey;

		  public CallableAnonymousInnerClass22(AuthorizationTest outerInstance, string processDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionKey = processDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.repositoryService.suspendProcessDefinitionByKey(processDefinitionKey);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendProcessDefinitionById(final String processDefinitionId)
	  protected internal virtual void suspendProcessDefinitionById(string processDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass23(this, processDefinitionId));
	  }

	  private class CallableAnonymousInnerClass23 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionId;

		  public CallableAnonymousInnerClass23(AuthorizationTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.repositoryService.suspendProcessDefinitionById(processDefinitionId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendProcessInstanceById(final String processInstanceId)
	  protected internal virtual void suspendProcessInstanceById(string processInstanceId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass24(this, processInstanceId));
	  }

	  private class CallableAnonymousInnerClass24 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processInstanceId;

		  public CallableAnonymousInnerClass24(AuthorizationTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.runtimeService.suspendProcessInstanceById(processInstanceId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobById(final String jobId)
	  protected internal virtual void suspendJobById(string jobId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass25(this, jobId));
	  }

	  private class CallableAnonymousInnerClass25 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string jobId;

		  public CallableAnonymousInnerClass25(AuthorizationTest outerInstance, string jobId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobId = jobId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobById(jobId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobByProcessInstanceId(final String processInstanceId)
	  protected internal virtual void suspendJobByProcessInstanceId(string processInstanceId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass26(this, processInstanceId));
	  }

	  private class CallableAnonymousInnerClass26 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processInstanceId;

		  public CallableAnonymousInnerClass26(AuthorizationTest outerInstance, string processInstanceId)
		  {
			  this.outerInstance = outerInstance;
			  this.processInstanceId = processInstanceId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobByProcessInstanceId(processInstanceId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobByJobDefinitionId(final String jobDefinitionId)
	  protected internal virtual void suspendJobByJobDefinitionId(string jobDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass27(this, jobDefinitionId));
	  }

	  private class CallableAnonymousInnerClass27 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string jobDefinitionId;

		  public CallableAnonymousInnerClass27(AuthorizationTest outerInstance, string jobDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobDefinitionId = jobDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobByJobDefinitionId(jobDefinitionId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobByProcessDefinitionId(final String processDefinitionId)
	  protected internal virtual void suspendJobByProcessDefinitionId(string processDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass28(this, processDefinitionId));
	  }

	  private class CallableAnonymousInnerClass28 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionId;

		  public CallableAnonymousInnerClass28(AuthorizationTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobByProcessDefinitionId(processDefinitionId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobByProcessDefinitionKey(final String processDefinitionKey)
	  protected internal virtual void suspendJobByProcessDefinitionKey(string processDefinitionKey)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass29(this, processDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass29 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionKey;

		  public CallableAnonymousInnerClass29(AuthorizationTest outerInstance, string processDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionKey = processDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobByProcessDefinitionKey(processDefinitionKey);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionById(final String jobDefinitionId)
	  protected internal virtual void suspendJobDefinitionById(string jobDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass30(this, jobDefinitionId));
	  }

	  private class CallableAnonymousInnerClass30 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string jobDefinitionId;

		  public CallableAnonymousInnerClass30(AuthorizationTest outerInstance, string jobDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobDefinitionId = jobDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionById(jobDefinitionId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionByProcessDefinitionId(final String processDefinitionId)
	  protected internal virtual void suspendJobDefinitionByProcessDefinitionId(string processDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass31(this, processDefinitionId));
	  }

	  private class CallableAnonymousInnerClass31 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionId;

		  public CallableAnonymousInnerClass31(AuthorizationTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionByProcessDefinitionKey(final String processDefinitionKey)
	  protected internal virtual void suspendJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass32(this, processDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass32 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionKey;

		  public CallableAnonymousInnerClass32(AuthorizationTest outerInstance, string processDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionKey = processDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinitionKey);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsById(final String jobDefinitionId)
	  protected internal virtual void suspendJobDefinitionIncludingJobsById(string jobDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass33(this, jobDefinitionId));
	  }

	  private class CallableAnonymousInnerClass33 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string jobDefinitionId;

		  public CallableAnonymousInnerClass33(AuthorizationTest outerInstance, string jobDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.jobDefinitionId = jobDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionById(jobDefinitionId, true);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsByProcessDefinitionId(final String processDefinitionId)
	  protected internal virtual void suspendJobDefinitionIncludingJobsByProcessDefinitionId(string processDefinitionId)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass34(this, processDefinitionId));
	  }

	  private class CallableAnonymousInnerClass34 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionId;

		  public CallableAnonymousInnerClass34(AuthorizationTest outerInstance, string processDefinitionId)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionId = processDefinitionId;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void suspendJobDefinitionIncludingJobsByProcessDefinitionKey(final String processDefinitionKey)
	  protected internal virtual void suspendJobDefinitionIncludingJobsByProcessDefinitionKey(string processDefinitionKey)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass35(this, processDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass35 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string processDefinitionKey;

		  public CallableAnonymousInnerClass35(AuthorizationTest outerInstance, string processDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.processDefinitionKey = processDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.managementService.suspendJobDefinitionByProcessDefinitionKey(processDefinitionKey, true);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.repository.Deployment createDeployment(final String name, final String... resources)
	  protected internal virtual Deployment createDeployment(string name, params string[] resources)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass36(this, resources));
	  }

	  private class CallableAnonymousInnerClass36 : Callable<Deployment>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string[] resources;

		  public CallableAnonymousInnerClass36(AuthorizationTest outerInstance, string[] resources)
		  {
			  this.outerInstance = outerInstance;
			  this.resources = resources;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.repository.Deployment call() throws Exception
		  public Deployment call()
		  {
			DeploymentBuilder builder = outerInstance.repositoryService.createDeployment();
			foreach (string resource in resources)
			{
			  builder.addClasspathResource(resource);
			}
			return builder.deploy();
		  }
	  }

	  protected internal virtual void deleteDeployment(string deploymentId)
	  {
		deleteDeployment(deploymentId, true);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteDeployment(final String deploymentId, final boolean cascade)
	  protected internal virtual void deleteDeployment(string deploymentId, bool cascade)
	  {
		Authentication authentication = identityService.CurrentAuthentication;
		try
		{
		  identityService.clearAuthentication();
		  runWithoutAuthorization(new CallableAnonymousInnerClass37(this, deploymentId, cascade));
		}
		finally
		{
		  if (authentication != null)
		  {
			identityService.Authentication = authentication;
		  }
		}
	  }

	  private class CallableAnonymousInnerClass37 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string deploymentId;
		  private bool cascade;

		  public CallableAnonymousInnerClass37(AuthorizationTest outerInstance, string deploymentId, bool cascade)
		  {
			  this.outerInstance = outerInstance;
			  this.deploymentId = deploymentId;
			  this.cascade = cascade;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			outerInstance.repositoryService.deleteDeployment(deploymentId, cascade);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.runtime.ProcessInstance startProcessAndExecuteJob(final String key)
	  protected internal virtual ProcessInstance startProcessAndExecuteJob(string key)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass38(this, key));
	  }

	  private class CallableAnonymousInnerClass38 : Callable<ProcessInstance>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string key;

		  public CallableAnonymousInnerClass38(AuthorizationTest outerInstance, string key)
		  {
			  this.outerInstance = outerInstance;
			  this.key = key;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.runtime.ProcessInstance call() throws Exception
		  public ProcessInstance call()
		  {
			ProcessInstance processInstance = outerInstance.startProcessInstanceByKey(key);
			outerInstance.executeAvailableJobs(key);
			return processInstance;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void executeAvailableJobs(final String key)
	  protected internal virtual void executeAvailableJobs(string key)
	  {
		runWithoutAuthorization(new CallableAnonymousInnerClass39(this, key));
	  }

	  private class CallableAnonymousInnerClass39 : Callable<Void>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string key;

		  public CallableAnonymousInnerClass39(AuthorizationTest outerInstance, string key)
		  {
			  this.outerInstance = outerInstance;
			  this.key = key;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Void call() throws Exception
		  public Void call()
		  {
			IList<Job> jobs = outerInstance.managementService.createJobQuery().processDefinitionKey(key).withRetriesLeft().list();

			if (jobs.Count == 0)
			{
			  outerInstance.enableAuthorization();
			  return null;
			}

			foreach (Job job in jobs)
			{
			  try
			  {
				outerInstance.managementService.executeJob(job.Id);
			  }
			  catch (Exception)
			  {
			  }
			}

			outerInstance.executeAvailableJobs(key);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected org.camunda.bpm.engine.repository.DecisionDefinition selectDecisionDefinitionByKey(final String decisionDefinitionKey)
	  protected internal virtual DecisionDefinition selectDecisionDefinitionByKey(string decisionDefinitionKey)
	  {
		return runWithoutAuthorization(new CallableAnonymousInnerClass40(this, decisionDefinitionKey));
	  }

	  private class CallableAnonymousInnerClass40 : Callable<DecisionDefinition>
	  {
		  private readonly AuthorizationTest outerInstance;

		  private string decisionDefinitionKey;

		  public CallableAnonymousInnerClass40(AuthorizationTest outerInstance, string decisionDefinitionKey)
		  {
			  this.outerInstance = outerInstance;
			  this.decisionDefinitionKey = decisionDefinitionKey;
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public org.camunda.bpm.engine.repository.DecisionDefinition call() throws Exception
		  public DecisionDefinition call()
		  {
			return outerInstance.repositoryService.createDecisionDefinitionQuery().decisionDefinitionKey(decisionDefinitionKey).singleResult();
		  }
	  }

	  // verify query results ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults<T1>(Query<T1> query, int countExpected)
	  {
		assertEquals(countExpected, query.list().Count);
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  protected internal virtual void verifySingleResultFails<T1>(Query<T1> query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  public virtual Permission DefaultTaskPermissionForUser
	  {
		  get
		  {
			// get the default task assignee permission
			ProcessEngineConfigurationImpl processEngineConfiguration = (ProcessEngineConfigurationImpl) processEngine.ProcessEngineConfiguration;
    
			return processEngineConfiguration.DefaultUserPermissionForTask;
		  }
	  }

	  // helper ////////////////////////////////////////////////////////////////////

	  protected internal virtual VariableMap Variables
	  {
		  get
		  {
			return Variables.createVariables().putValue(VARIABLE_NAME, VARIABLE_VALUE);
		  }
	  }
	}

}