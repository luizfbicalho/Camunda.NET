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


	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using MissingAuthorization = org.camunda.bpm.engine.authorization.MissingAuthorization;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using DurationReportResult = org.camunda.bpm.engine.history.DurationReportResult;
	using CleanableHistoricProcessInstanceReportResult = org.camunda.bpm.engine.history.CleanableHistoricProcessInstanceReportResult;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using PeriodUnit = org.camunda.bpm.engine.query.PeriodUnit;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	public class HistoricProcessInstanceAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";
	  protected internal const string MESSAGE_START_PROCESS_KEY = "messageStartProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/messageStartEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // historic process instance query //////////////////////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		HistoricProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);

		HistoricProcessInstance instance = query.singleResult();
		assertNotNull(instance);
		assertEquals(processInstanceId, instance.Id);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic process instance query (multiple process instances) ////////////////////////

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
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

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
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

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
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

		// then
		verifyQueryResults(query, 7);
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
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery();

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

	  // delete historic process instance //////////////////////////////

	  public virtual void testDeleteHistoricProcessInstanceWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteHistoricProcessInstance(processInstanceId);
		  fail("Exception expected: It should not be possible to delete the historic process instance");
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

	  public virtual void testDeleteHistoricProcessInstanceWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, DELETE_HISTORY);

		// when
		historyService.deleteHistoricProcessInstance(processInstanceId);

		// then
		disableAuthorization();
		long count = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).count();
		assertEquals(0, count);
		enableAuthorization();
	  }

	  public virtual void testDeleteHistoricProcessInstanceWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		// when
		historyService.deleteHistoricProcessInstance(processInstanceId);

		// then
		disableAuthorization();
		long count = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).count();
		assertEquals(0, count);
		enableAuthorization();
	  }

	  public virtual void testDeleteHistoricProcessInstanceAfterDeletingDeployment()
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
		historyService.deleteHistoricProcessInstance(processInstanceId);

		// then
		disableAuthorization();
		long count = historyService.createHistoricProcessInstanceQuery().processInstanceId(processInstanceId).count();
		assertEquals(0, count);
		enableAuthorization();
	  }

	  // create historic process instance report

	  public virtual void testHistoricProcessInstanceReportWithoutAuthorization()
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
		  historyService.createHistoricProcessInstanceReport().duration(PeriodUnit.MONTH);
		  fail("Exception expected: It should not be possible to create a historic process instance report");
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

	  public virtual void testHistoricProcessInstanceReportWithHistoryReadPermissionOnAny()
	  {
		// given
		startProcessInstanceByKey(PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(PeriodUnit.MONTH);

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testReportWithoutQueryCriteriaAndAnyReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, "*", userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().duration(PeriodUnit.MONTH);

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testReportWithoutQueryCriteriaAndNoReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		// when
		try
		{
		  historyService.createHistoricProcessInstanceReport().duration(PeriodUnit.MONTH);

		  // then
		  fail("Exception expected: It should not be possible to create a historic process instance report");
		}
		catch (AuthorizationException)
		{

		}
	  }

	  public virtual void testReportWithQueryCriterionProcessDefinitionKeyInAndReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_KEY, MESSAGE_START_PROCESS_KEY).duration(PeriodUnit.MONTH);

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testReportWithQueryCriterionProcessDefinitionKeyInAndMissingReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		try
		{
		  historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_KEY, MESSAGE_START_PROCESS_KEY).duration(PeriodUnit.MONTH);

		  // then
		  fail("Exception expected: It should not be possible to create a historic process instance report");
		}
		catch (AuthorizationException)
		{

		}
	  }

	  public virtual void testReportWithQueryCriterionProcessDefinitionIdInAndReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processInstance1.ProcessDefinitionId, processInstance2.ProcessDefinitionId).duration(PeriodUnit.MONTH);

		// then
		assertEquals(1, result.Count);
	  }

	  public virtual void testReportWithQueryCriterionProcessDefinitionIdInAndMissingReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		try
		{
		  historyService.createHistoricProcessInstanceReport().processDefinitionIdIn(processInstance1.ProcessDefinitionId, processInstance2.ProcessDefinitionId).duration(PeriodUnit.MONTH);

		  // then
		  fail("Exception expected: It should not be possible to create a historic process instance report");
		}
		catch (AuthorizationException)
		{

		}
	  }

	  public virtual void testReportWithMixedQueryCriteriaAndReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, MESSAGE_START_PROCESS_KEY, userId, READ_HISTORY);

		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_KEY).processDefinitionIdIn(processInstance2.ProcessDefinitionId).duration(PeriodUnit.MONTH);

		// then
		assertEquals(0, result.Count);
	  }

	  public virtual void testReportWithMixedQueryCriteriaAndMissingReadHistoryPermission()
	  {
		// given
		ProcessInstance processInstance1 = startProcessInstanceByKey(PROCESS_KEY);
		ProcessInstance processInstance2 = startProcessInstanceByKey(MESSAGE_START_PROCESS_KEY);
		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstance1.ProcessInstanceId, "");
		runtimeService.deleteProcessInstance(processInstance2.ProcessInstanceId, "");
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		try
		{
		historyService.createHistoricProcessInstanceReport().processDefinitionKeyIn(PROCESS_KEY).processDefinitionIdIn(processInstance2.ProcessDefinitionId).duration(PeriodUnit.MONTH);

		  // then
		  fail("Exception expected: It should not be possible to create a historic process instance report");
		}
		catch (AuthorizationException)
		{

		}
	  }

	  public virtual void testReportWithQueryCriterionProcessInstanceIdInWrongProcessDefinitionId()
	  {
		// when
		IList<DurationReportResult> result = historyService.createHistoricProcessInstanceReport().processDefinitionIdIn("aWrongProcessDefinitionId").duration(PeriodUnit.MONTH);

		// then
		assertEquals(0, result.Count);
	  }

	  public virtual void testHistoryCleanupReportWithPermissions()
	  {
		// given
		prepareProcessInstances(PROCESS_KEY, -6, 5, 10);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, Permissions.READ, Permissions.READ_HISTORY);
		createGrantAuthorizationGroup(PROCESS_DEFINITION, PROCESS_KEY, groupId, Permissions.READ, Permissions.READ_HISTORY);

		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(1, reportResults.Count);
		assertEquals(10, reportResults[0].CleanableProcessInstanceCount);
		assertEquals(10, reportResults[0].FinishedProcessInstanceCount);
	  }

	  public virtual void testHistoryCleanupReportWithReadPermissionOnly()
	  {
		// given
		prepareProcessInstances(PROCESS_KEY, -6, 5, 10);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, Permissions.READ);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  public virtual void testHistoryCleanupReportWithReadHistoryPermissionOnly()
	  {
		// given
		prepareProcessInstances(PROCESS_KEY, -6, 5, 10);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, Permissions.READ_HISTORY);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  public virtual void testHistoryCleanupReportWithoutPermissions()
	  {
		// given
		prepareProcessInstances(PROCESS_KEY, -6, 5, 10);

		// when
		IList<CleanableHistoricProcessInstanceReportResult> reportResults = historyService.createCleanableHistoricProcessInstanceReport().list();

		// then
		assertEquals(0, reportResults.Count);
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricProcessInstanceQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void prepareProcessInstances(string key, int daysInThePast, int? historyTimeToLive, int instanceCount)
	  {
		ProcessDefinition processDefinition = selectProcessDefinitionByKey(key);
		disableAuthorization();
		repositoryService.updateProcessDefinitionHistoryTimeToLive(processDefinition.Id, historyTimeToLive);
		enableAuthorization();

		DateTime oldCurrentTime = ClockUtil.CurrentTime;
		ClockUtil.CurrentTime = DateUtils.addDays(DateTime.Now, daysInThePast);

		IList<string> processInstanceIds = new List<string>();
		for (int i = 0; i < instanceCount; i++)
		{
		  ProcessInstance processInstance = startProcessInstanceByKey(key);
		  processInstanceIds.Add(processInstance.Id);
		}

		disableAuthorization();
		runtimeService.deleteProcessInstances(processInstanceIds, null, true, true);
		enableAuthorization();

		ClockUtil.CurrentTime = oldCurrentTime;
	  }

	}

}