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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.TaskPermissions.READ_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_INSTANCE_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using VariableInstanceQuery = org.camunda.bpm.engine.runtime.VariableInstanceQuery;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class VariableInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";

	  protected internal new string deploymentId;
	  protected internal bool ensureSpecificVariablePermission;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;
		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
	  }

	  public virtual void testProcessVariableQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testCaseVariableQueryWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testCaseLocalTaskVariableQueryWithoutAuthorization()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testStandaloneTaskVariableQueryWithoutAuthorization()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);

		deleteTask(taskId, true);
	  }

	  public virtual void testProcessVariableQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessVariableQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessVariableQueryWithReadInstanceVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  // CAM-9888
	  public virtual void failingTestProcessVariableQueryWithReadVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(TASK, ANY, userId, READ_VARIABLE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testProcessVariableQueryWithReadProcessInstanceWhenReadVariableIsEnabled()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testProcessVariableQueryWithReadTaskWhenReadVariableIsEnabled()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(PROCESS_KEY, Variables).Id;
		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnTask()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ);
		createGrantAuthorization(TASK, ANY, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadPermissionOnOneProcessTask()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadInstanceVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadVariablePermission()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		VariableInstance variable = query.singleResult();
		assertNotNull(variable);
		assertEquals(processInstanceId, variable.ProcessInstanceId);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadProcessInstanceWhenReadVariableIsEnabled()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testProcessLocalTaskVariableQueryWithReadTaskWhenReadVariableIsEnabled()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setTaskVariableLocal(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testStandaloneTaskVariableQueryWithReadPermissionOnTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testStandaloneTaskVariableQueryWithReadVariablePermissionOnTask()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string taskId = "myTask";
		createTask(taskId);
		setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// when
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testMixedVariables()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY, Variables).ProcessInstanceId;

		createCaseInstanceByKey(CASE_KEY, Variables);

		// when (1)
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then (1)
		verifyQueryResults(query, 1);

		// when (2)
		createGrantAuthorization(TASK, taskId, userId, READ);

		// then (2)
		verifyQueryResults(query, 2);

		// when (3)
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// then (3)
		verifyQueryResults(query, 3);

		deleteTask(taskId, true);
	  }

	  public virtual void testMixedVariablesWhenReadVariableIsEnabled()
	  {
		// given
		setReadVariableAsDefaultReadVariablePermission();
		string taskId = "myTask";
		createTask(taskId);
		setTaskVariable(taskId, VARIABLE_NAME, VARIABLE_VALUE);

		startProcessInstanceByKey(PROCESS_KEY, Variables).ProcessInstanceId;

		createCaseInstanceByKey(CASE_KEY, Variables);

		// when (1)
		VariableInstanceQuery query = runtimeService.createVariableInstanceQuery();

		// then (1)
		verifyQueryResults(query, 1);

		// when (2)
		createGrantAuthorization(TASK, taskId, userId, READ_VARIABLE);

		// then (2)
		verifyQueryResults(query, 2);

		// when (3)
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_INSTANCE_VARIABLE);

		// then (3)
		verifyQueryResults(query, 3);

		deleteTask(taskId, true);
	  }

	  // helper ////////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(VariableInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void setReadVariableAsDefaultReadVariablePermission()
	  {
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }

	}

}