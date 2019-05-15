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
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.TASK;

	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using HistoricTaskInstanceReportResult = org.camunda.bpm.engine.history.HistoricTaskInstanceReportResult;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoricTaskInstanceAuthorizationTest : AuthorizationTest
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

	  // historic task instance query (standalone task) ///////////////////////////////////////

	  public virtual void testQueryAfterStandaloneTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  // historic task instance query (process task) //////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic task instance query (multiple process instances) ////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  // historic task instance query (case task) ///////////////////////////////////////

	  public virtual void testQueryAfterCaseTask()
	  {
		// given
		createCaseInstanceByKey(CASE_KEY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic task instance query (mixed tasks) ////////////////////////////////////

	  public virtual void testMixedQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createTask("one");
		createTask("two");
		createTask("three");
		createTask("four");
		createTask("five");

		createCaseInstanceByKey(CASE_KEY);
		createCaseInstanceByKey(CASE_KEY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

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
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createTask("one");
		createTask("two");
		createTask("three");
		createTask("four");
		createTask("five");

		createCaseInstanceByKey(CASE_KEY);
		createCaseInstanceByKey(CASE_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

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
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);

		createTask("one");
		createTask("two");
		createTask("three");
		createTask("four");
		createTask("five");

		createCaseInstanceByKey(CASE_KEY);
		createCaseInstanceByKey(CASE_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

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
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
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
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery();

		// then
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // delete historic task (standalone task) ///////////////////////

	  public virtual void testDeleteStandaloneTask()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		disableAuthorization();
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().taskId(taskId);
		verifyQueryResults(query, 0);
		enableAuthorization();

		deleteTask(taskId, true);
	  }

	  // delete historic task (process task) ///////////////////////

	  public virtual void testDeleteProcessTaskWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		try
		{
		  // when
		  historyService.deleteHistoricTaskInstance(taskId);
		  fail("Exception expected: It should not be possible to delete the historic task instance");
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

	  public virtual void testDeleteProcessTaskWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_HISTORY);

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		disableAuthorization();
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().taskId(taskId);
		verifyQueryResults(query, 0);
		enableAuthorization();
	  }

	  public virtual void testDeleteProcessTaskWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		disableAuthorization();
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().taskId(taskId);
		verifyQueryResults(query, 0);
		enableAuthorization();
	  }

	  public virtual void testDeleteHistoricTaskInstanceAfterDeletingDeployment()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		// when
		historyService.deleteHistoricTaskInstance(taskId);

		// then
		disableAuthorization();
		HistoricTaskInstanceQuery query = historyService.createHistoricTaskInstanceQuery().taskId(taskId);
		verifyQueryResults(query, 0);
		enableAuthorization();

		disableAuthorization();
		historyService.deleteHistoricProcessInstance(processInstanceId);
		enableAuthorization();
	  }

	  public virtual void testHistoricTaskInstanceReportWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		try
		{
		  // when
		  historyService.createHistoricTaskInstanceReport().duration(PeriodUnit.MONTH);
		  fail("Exception expected: It should not be possible to create a historic task instance report");
		}
		catch (AuthorizationException e)
		{
		  // then
		  IList<MissingAuthorization> missingAuthorizations = e.MissingAuthorizations;
		  assertEquals(1, missingAuthorizations.Count);

		  MissingAuthorization missingAuthorization = missingAuthorizations[0];
		  assertEquals(READ_HISTORY.ToString(), missingAuthorization.ViolatedPermissionName);
		  assertEquals(PROCESS_DEFINITION.resourceName(), missingAuthorization.ResourceType);
		  assertEquals(ANY, missingAuthorization.ResourceId);
		}
	  }

	  public virtual void testHistoricTaskInstanceReportWithHistoryReadPermissionOnAny()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(TASK, ANY, userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricTaskInstanceReport().duration(PeriodUnit.MONTH);

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testHistoricTaskInstanceReportGroupedByProcessDefinitionKeyWithHistoryReadPermissionOnAny()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(TASK, ANY, userId, READ_HISTORY);

		// when
		IList<HistoricTaskInstanceReportResult> result = historyService.createHistoricTaskInstanceReport().countByProcessDefinitionKey();

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testHistoricTaskInstanceReportGroupedByTaskNameWithHistoryReadPermissionOnAny()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(TASK, ANY, userId, READ_HISTORY);

		// when
		IList<HistoricTaskInstanceReportResult> result = historyService.createHistoricTaskInstanceReport().countByTaskName();

		// then
		assertEquals(1, result.Count);
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricTaskInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}