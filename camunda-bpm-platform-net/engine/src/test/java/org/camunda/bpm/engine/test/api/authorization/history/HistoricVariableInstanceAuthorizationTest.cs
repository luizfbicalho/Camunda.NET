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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions.READ_HISTORY_VARIABLE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricVariableInstanceQuery = org.camunda.bpm.engine.history.HistoricVariableInstanceQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricVariableInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";

	  protected internal new string deploymentId;
	  protected internal bool ensureSpecificVariablePermission;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;

		ensureSpecificVariablePermission = processEngineConfiguration.EnforceSpecificVariablePermission;

		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);

		processEngineConfiguration.EnforceSpecificVariablePermission = ensureSpecificVariablePermission;
	  }

	  // historic variable instance query (standalone task) /////////////////////////////////////////////

	  public virtual void testQueryAfterStandaloneTaskVariables()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		taskService.setVariables(taskId, Variables);
		enableAuthorization();

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  // historic variable instance query (process variables) ///////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadHistoryVariablePermissionOnProcessDefinition()
	  {
		// given
		setReadHistoryVariableAsDefaultReadPermission();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadHistoryVariablePermissionOnAnyProcessDefinition()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultipleReadHistoryVariable()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY_VARIABLE);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic variable instance query (multiple process instances) ////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		startMultipleProcessInstances();

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		startMultipleProcessInstances();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startMultipleProcessInstances();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithReadHistoryVariablePermissionOnProcessDefinition()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startMultipleProcessInstances();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadHistoryVariablePermissionOnAnyProcessDefinition()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startMultipleProcessInstances();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  // historic variable instance query (case variables) /////////////////////////////////////////////

	  public virtual void testQueryAfterCaseVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic variable instance query (mixed variables) ////////////////////////////////////

	  public virtual void testMixedQueryWithoutAuthorization()
	  {
		startMultipleProcessInstances();

		setupMultipleMixedVariables();

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 7);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  public virtual void testMixedQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		startMultipleProcessInstances();

		setupMultipleMixedVariables();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 10);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  public virtual void testMixedQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		startMultipleProcessInstances();

		setupMultipleMixedVariables();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 14);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  public virtual void testMixedQueryWithReadHistoryVariablePermissionOnProcessDefinition()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startMultipleProcessInstances();

		setupMultipleMixedVariables();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 10);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  public virtual void testMixedQueryWithReadHistoryVariablePermissionOnAnyProcessDefinition()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startMultipleProcessInstances();

		setupMultipleMixedVariables();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY_VARIABLE);

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 14);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		enableAuthorization();

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 3);

		cleanUpAfterDeploymentDeletion();
	  }

	  public virtual void testQueryAfterDeletingDeploymentWithReadHistoryVariable()
	  {
		setReadHistoryVariableAsDefaultReadPermission();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY_VARIABLE);

		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		enableAuthorization();

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		// when
		HistoricVariableInstanceQuery query = historyService.createHistoricVariableInstanceQuery();

		// then
		verifyQueryResults(query, 3);

		cleanUpAfterDeploymentDeletion();
	  }

	  // delete historic variable instance (process variables) /////////////////////////////////////////////
	  public virtual void testDeleteHistoricProcessVariableInstanceWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		disableAuthorization();
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstance(variableInstanceId);
		  fail("Exception expected: It should not be possible to delete the historic variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE_HISTORY.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteHistoricProcessVariableInstanceWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_HISTORY);

		disableAuthorization();
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstance(variableInstanceId);
		}
		catch (AuthorizationException)
		{
		  fail("It should be possible to delete the historic variable instance with granted permissions");
		}
		// then
		verifyVariablesDeleted();
	  }

	  // delete deployment (cascade = false)
	  public virtual void testDeleteHistoricProcessVariableInstanceAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstance(variableInstanceId);
		}
		catch (AuthorizationException)
		{
		  fail("It should be possible to delete the historic variable instance with granted permissions after the process definition is deleted");
		}
		// then
		verifyVariablesDeleted();
		cleanUpAfterDeploymentDeletion();
	  }

	  // delete historic variable instance (case variables) /////////////////////////////////////////////
	  public virtual void testDeleteHistoricCaseVariableInstance()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);

		disableAuthorization();
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyVariablesDeleted();
	  }

	  // delete historic variable instance (task variables) /////////////////////////////////////////////
	  public virtual void testDeleteHistoricStandaloneTaskVariableInstance()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		taskService.setVariables(taskId, Variables);
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyVariablesDeleted();
		deleteTask(taskId, true);

		// XXX if CAM-6570 is implemented, there should be a check for variables of standalone tasks here as well
	  }

	  // delete historic variable instances (process variables) /////////////////////////////////////////////
	  public virtual void testDeleteHistoricProcessVariableInstancesWithoutAuthorization()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY, Variables);
		verifyVariablesCreated();

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId(instance.Id);
		  fail("Exception expected: It should not be possible to delete the historic variable instance");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE_HISTORY.Name, message);
		  assertTextPresent(PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteHistoricProcessVariableInstancesWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY, Variables);
		verifyVariablesCreated();
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_HISTORY);

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId(instance.Id);
		}
		catch (AuthorizationException)
		{
		  fail("It should be possible to delete the historic variable instance with granted permissions");
		}
		// then
		verifyVariablesDeleted();
	  }

	  // delete deployment (cascade = false)
	  public virtual void testDeleteHistoricProcessVariableInstancesAfterDeletingDeployment()
	  {
		// given
		ProcessInstance instance = startProcessInstanceByKey(PROCESS_KEY, Variables);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		verifyVariablesCreated();
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteHistoricVariableInstancesByProcessInstanceId(instance.Id);
		}
		catch (AuthorizationException)
		{
		  fail("It should be possible to delete the historic variable instance with granted permissions after the process definition is deleted");
		}

		// then
		verifyVariablesDeleted();
		cleanUpAfterDeploymentDeletion();
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricVariableInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyVariablesDeleted()
	  {
		disableAuthorization();
		assertEquals(0L, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(0L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();
	  }

	  protected internal virtual void verifyVariablesCreated()
	  {
		disableAuthorization();
		assertEquals(1L, historyService.createHistoricVariableInstanceQuery().count());
		assertEquals(1L, historyService.createHistoricDetailQuery().count());
		enableAuthorization();
	  }

	  protected internal virtual void cleanUpAfterDeploymentDeletion()
	  {
		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  protected internal virtual void startMultipleProcessInstances()
	  {
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
	  }

	  protected internal virtual void setupMultipleMixedVariables()
	  {
		createTask("one");
		createTask("two");
		createTask("three");
		createTask("four");
		createTask("five");

		disableAuthorization();
		taskService.setVariables("one", Variables);
		taskService.setVariables("two", Variables);
		taskService.setVariables("three", Variables);
		taskService.setVariables("four", Variables);
		taskService.setVariables("five", Variables);
		enableAuthorization();

		createCaseInstanceByKey(CASE_KEY, Variables);
		createCaseInstanceByKey(CASE_KEY, Variables);
	  }

	  protected internal virtual void setReadHistoryVariableAsDefaultReadPermission()
	  {
		processEngineConfiguration.EnforceSpecificVariablePermission = true;
	  }

	}

}