using System;

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
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.CREATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using StartFormData = org.camunda.bpm.engine.form.StartFormData;
	using TaskFormData = org.camunda.bpm.engine.form.TaskFormData;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using ExecuteCommandDelegate = org.camunda.bpm.engine.test.api.authorization.service.ExecuteCommandDelegate;
	using ExecuteCommandListener = org.camunda.bpm.engine.test.api.authorization.service.ExecuteCommandListener;
	using ExecuteCommandTaskListener = org.camunda.bpm.engine.test.api.authorization.service.ExecuteCommandTaskListener;
	using ExecuteQueryDelegate = org.camunda.bpm.engine.test.api.authorization.service.ExecuteQueryDelegate;
	using ExecuteQueryListener = org.camunda.bpm.engine.test.api.authorization.service.ExecuteQueryListener;
	using ExecuteQueryTaskListener = org.camunda.bpm.engine.test.api.authorization.service.ExecuteQueryTaskListener;
	using MyDelegationService = org.camunda.bpm.engine.test.api.authorization.service.MyDelegationService;
	using MyFormFieldValidator = org.camunda.bpm.engine.test.api.authorization.service.MyFormFieldValidator;
	using MyServiceTaskActivityBehaviorExecuteCommand = org.camunda.bpm.engine.test.api.authorization.service.MyServiceTaskActivityBehaviorExecuteCommand;
	using MyServiceTaskActivityBehaviorExecuteQuery = org.camunda.bpm.engine.test.api.authorization.service.MyServiceTaskActivityBehaviorExecuteQuery;
	using MyTaskService = org.camunda.bpm.engine.test.api.authorization.service.MyTaskService;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class DelegationAuthorizationTest : AuthorizationTest
	{

	  public const string DEFAULT_PROCESS_KEY = "process";

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		MyDelegationService.clearProperties();
		processEngineConfiguration.AuthorizationEnabledForCustomCode = false;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesQueryAfterUserCompletesTask()
	  public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesCommandAfterUserCompletesTask()
	  public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTask()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myDelegate"] = new ExecuteQueryDelegate();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myDelegate"] = new ExecuteCommandDelegate();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsExpression()
	  public virtual void testJavaDelegateExecutesQueryAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myDelegate"] = new ExecuteQueryDelegate();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsExpression()
	  public virtual void testJavaDelegateExecutesCommandAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myDelegate"] = new ExecuteCommandDelegate();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTask()
	  public virtual void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTask()
	  public virtual void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTask()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testCustomActivityBehaviorExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myBehavior"] = new MyServiceTaskActivityBehaviorExecuteQuery();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testCustomActivityBehaviorExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myBehavior"] = new MyServiceTaskActivityBehaviorExecuteCommand();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignallableActivityBehaviorAsClass()
	  public virtual void testSignallableActivityBehaviorAsClass()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 4);
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.signal(processInstanceId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testSignallableActivityBehaviorAsDelegateExpression()
	  public virtual void testSignallableActivityBehaviorAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["activityBehavior"] = new MyServiceTaskActivityBehaviorExecuteQuery();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 4);
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		runtimeService.signal(processInstanceId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesQueryAfterUserCompletesTask()
	  public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesCommandAfterUserCompletesTask()
	  public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTask()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteQueryListener();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteCommandListener();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsExpression()
	  public virtual void testExecutionListenerExecutesQueryAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteQueryListener();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsExpression()
	  public virtual void testExecutionListenerExecutesCommandAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteCommandListener();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesQueryAfterUserCompletesTask()
	  public virtual void testTaskListenerExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesCommandAfterUserCompletesTask()
	  public virtual void testTaskListenerExecutesCommandAfterUserCompletesTask()
	  {
		// given
		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testTaskListenerExecutesQueryAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteQueryTaskListener();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  public virtual void testTaskListenerExecutesCommandAfterUserCompletesTaskAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteCommandTaskListener();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesQueryAfterUserCompletesTaskAsExpression()
	  public virtual void testTaskListenerExecutesQueryAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteQueryTaskListener();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerExecutesCommandAfterUserCompletesTaskAsExpression()
	  public virtual void testTaskListenerExecutesCommandAfterUserCompletesTaskAsExpression()
	  {
		// given
		processEngineConfiguration.Beans["myListener"] = new ExecuteCommandTaskListener();

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(2, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskAssigneeExpression()
	  public virtual void testTaskAssigneeExpression()
	  {
		// given
		processEngineConfiguration.Beans["myTaskService"] = new MyTaskService();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptTaskExecutesQueryAfterUserCompletesTask()
	  public virtual void testScriptTaskExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		Task task = selectAnyTask();

		string taskId = task.Id;
		string processInstanceId = task.ProcessInstanceId;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(5l, variableCount.Value);

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptTaskExecutesCommandAfterUserCompletesTask()
	  public virtual void testScriptTaskExecutesCommandAfterUserCompletesTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstance variableUser = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		assertEquals(2, runtimeService.createProcessInstanceQuery().count());

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptExecutionListenerExecutesQueryAfterUserCompletesTask()
	  public virtual void testScriptExecutionListenerExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		Task task = selectAnyTask();

		string taskId = task.Id;
		string processInstanceId = task.ProcessInstanceId;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(5l, variableCount.Value);

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptExecutionListenerExecutesCommandAfterUserCompletesTask()
	  public virtual void testScriptExecutionListenerExecutesCommandAfterUserCompletesTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstance variableUser = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		assertEquals(2, runtimeService.createProcessInstanceQuery().count());

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptTaskListenerExecutesQueryAfterUserCompletesTask()
	  public virtual void testScriptTaskListenerExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		Task task = selectAnyTask();

		string taskId = task.Id;
		string processInstanceId = task.ProcessInstanceId;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(5l, variableCount.Value);

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptTaskListenerExecutesCommandAfterUserCompletesTask()
	  public virtual void testScriptTaskListenerExecutesCommandAfterUserCompletesTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstance variableUser = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		assertEquals(2, runtimeService.createProcessInstanceQuery().count());

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptConditionExecutesQueryAfterUserCompletesTask()
	  public virtual void testScriptConditionExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		Task task = selectAnyTask();

		string taskId = task.Id;
		string processInstanceId = task.ProcessInstanceId;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(5l, variableCount.Value);

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptConditionExecutesCommandAfterUserCompletesTask()
	  public virtual void testScriptConditionExecutesCommandAfterUserCompletesTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstance variableUser = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId).variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		assertEquals(2, runtimeService.createProcessInstanceQuery().count());

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptIoMappingExecutesQueryAfterUserCompletesTask()
	  public virtual void testScriptIoMappingExecutesQueryAfterUserCompletesTask()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		Task task = selectAnyTask();

		string taskId = task.Id;
		string processInstanceId = task.ProcessInstanceId;

		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(5l, variableCount.Value);

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testScriptIoMappingExecutesCommandAfterUserCompletesTask()
	  public virtual void testScriptIoMappingExecutesCommandAfterUserCompletesTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		disableAuthorization();

		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery().processInstanceIdIn(processInstanceId);

		VariableInstance variableUser = query.variableName("userId").singleResult();
		assertNotNull(variableUser);
		assertEquals(userId, variableUser.Value);

		VariableInstance variableCount = query.variableName("count").singleResult();
		assertNotNull(variableCount);
		assertEquals(1l, variableCount.Value);

		assertEquals(2, runtimeService.createProcessInstanceQuery().count());

		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomStartFormHandlerExecutesQuery()
	  public virtual void testCustomStartFormHandlerExecutesQuery()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string processDefinitionId = selectProcessDefinitionByKey(DEFAULT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, READ);

		// when
		StartFormData startFormData = formService.getStartFormData(processDefinitionId);

		// then
		assertNotNull(startFormData);

		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomTaskFormHandlerExecutesQuery()
	  public virtual void testCustomTaskFormHandlerExecutesQuery()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		TaskFormData taskFormData = formService.getTaskFormData(taskId);

		// then
		assertNotNull(taskFormData);

		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/authorization/DelegationAuthorizationTest.testCustomStartFormHandlerExecutesQuery.bpmn20.xml"})]
	  public virtual void testSubmitCustomStartFormHandlerExecutesQuery()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string processDefinitionId = selectProcessDefinitionByKey(DEFAULT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, DEFAULT_PROCESS_KEY, userId, CREATE_INSTANCE);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, CREATE);

		// when
		formService.submitStartForm(processDefinitionId, null);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/authorization/DelegationAuthorizationTest.testCustomTaskFormHandlerExecutesQuery.bpmn20.xml"})]
	  public virtual void testSubmitCustomTaskFormHandlerExecutesQuery()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomFormFieldValidator()
	  public virtual void testCustomFormFieldValidator()
	  {
		// given
		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCustomFormFieldValidatorAsDelegateExpression()
	  public virtual void testCustomFormFieldValidatorAsDelegateExpression()
	  {
		// given
		processEngineConfiguration.Beans["myValidator"] = new MyFormFieldValidator();

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);

		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		formService.submitTaskForm(taskId, null);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(5), MyDelegationService.INSTANCES_COUNT);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/authorization/DelegationAuthorizationTest.testJavaDelegateExecutesQueryAfterUserCompletesTask.bpmn20.xml"})]
	  public virtual void testPerformAuthorizationCheckByExecutingQuery()
	  {
		// given
		processEngineConfiguration.AuthorizationEnabledForCustomCode = true;

		startProcessInstancesByKey(DEFAULT_PROCESS_KEY, 5);
		string taskId = selectAnyTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when
		taskService.complete(taskId);

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		assertEquals(Convert.ToInt64(0), MyDelegationService.INSTANCES_COUNT);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/authorization/DelegationAuthorizationTest.testJavaDelegateExecutesCommandAfterUserCompletesTask.bpmn20.xml"})]
	  public virtual void testPerformAuthorizationCheckByExecutingCommand()
	  {
		// given
		processEngineConfiguration.AuthorizationEnabledForCustomCode = true;

		startProcessInstanceByKey(DEFAULT_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		try
		{
		  // when
		  taskService.complete(taskId);
		  fail("Exception expected: It should not be possible to execute the command inside JavaDelegate");
		}
		catch (AuthorizationException)
		{
		}

		// then
		assertNotNull(MyDelegationService.CURRENT_AUTHENTICATION);
		assertEquals(userId, MyDelegationService.CURRENT_AUTHENTICATION.UserId);

		disableAuthorization();
		assertEquals(1, runtimeService.createProcessInstanceQuery().count());
		enableAuthorization();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testTaskListenerOnCreateAssignsTask()
	  public virtual void testTaskListenerOnCreateAssignsTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(DEFAULT_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(TASK, taskId, userId, UPDATE);

		// when (1)
		taskService.complete(taskId);

		// then (1)
		identityService.clearAuthentication();
		identityService.setAuthentication("demo", null);

		Task task = taskService.createTaskQuery().singleResult();
		assertNotNull(task);

		// when (2)
		taskService.complete(task.Id);

		// then (2)
		assertProcessEnded(processInstanceId);
	  }

	  // helper /////////////////////////////////////////////////////////////////////////

	  protected internal virtual void startProcessInstancesByKey(string key, int count)
	  {
		for (int i = 0; i < count; i++)
		{
		  startProcessInstanceByKey(key);
		}
	  }

	  protected internal virtual Task selectAnyTask()
	  {
		disableAuthorization();
		Task task = taskService.createTaskQuery().listPage(0, 1).get(0);
		enableAuthorization();
		return task;
	  }

	}

}