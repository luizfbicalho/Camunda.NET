﻿using System.Collections.Generic;

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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using HistoricDetailQuery = org.camunda.bpm.engine.history.HistoricDetailQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricDetailAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";
	  protected internal const string CASE_KEY = "oneTaskCase";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // historic variable update query (standalone task) /////////////////////////////////////////////

	  public virtual void testQueryAfterStandaloneTaskVariableUpdates()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		disableAuthorization();
		taskService.setVariables(taskId, Variables);
		enableAuthorization();

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  // historic variable update query (process task) /////////////////////////////////////////////

	  public virtual void testSimpleVariableUpdateQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleVariableUpdateQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleVariableUpdateQueryMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleVariableUpdateQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic variable update query (multiple process instances) ///////////////////////////////////////////

	  public virtual void testVariableUpdateQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testVariableUpdateQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testVariableUpdateQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 7);
	  }

	  // historic variable update query (case variables) /////////////////////////////////////////////

	  public virtual void testQueryAfterCaseVariables()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY, Variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic variable update query (mixed) ////////////////////////////////////

	  public virtual void testMixedQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

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

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

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
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

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

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

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
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

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

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().variableUpdates();

		// then
		verifyQueryResults(query, 14);

		deleteTask("one", true);
		deleteTask("two", true);
		deleteTask("three", true);
		deleteTask("four", true);
		deleteTask("five", true);
	  }

	  // historic form field query //////////////////////////////////////////////////////

	  public virtual void testSimpleFormFieldQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleFormFieldQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleFormFieldQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic variable update query (multiple process instances) ///////////////////////////////////////////

	  public virtual void testFormFieldQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY, Variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testFormFieldQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testFormFieldQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery().formFields();

		// then
		verifyQueryResults(query, 7);
	  }

	  // historic detail query (variable update + form field) //////////

	  public virtual void testDetailQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testDetailQueryWithReadHistoryOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testDetailQueryWithReadHistoryOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);
		startProcessInstanceByKey(PROCESS_KEY, Variables);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

		startProcessInstanceByKey(PROCESS_KEY);
		taskId = selectSingleTask().Id;
		disableAuthorization();
		formService.submitTaskForm(taskId, Variables);
		enableAuthorization();

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
		HistoricDetailQuery query = historyService.createHistoricDetailQuery();

		// then
		verifyQueryResults(query, 7);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricDetailQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}