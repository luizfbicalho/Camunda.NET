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
namespace org.camunda.bpm.engine.test.history
{
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using HistoricExternalTaskLog = org.camunda.bpm.engine.history.HistoricExternalTaskLog;
	using HistoricExternalTaskLogQuery = org.camunda.bpm.engine.history.HistoricExternalTaskLogQuery;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using TestOrderingUtil = org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


	using static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.DEFAULT_TOPIC;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.models.builder.DefaultExternalTaskModelBuilder.createDefaultExternalTaskModel;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricExternalTaskLogQuerySortingTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoricExternalTaskLogQuerySortingTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			authRule = new AuthorizationTestRule(engineRule);
			testHelper = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
		}


	  protected internal readonly string WORKER_ID = "aWorkerId";
	  protected internal readonly long LOCK_DURATION = 5 * 60L * 1000L;

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal AuthorizationTestRule authRule;
	  protected internal ProcessEngineTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(authRule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public readonly ExpectedException thrown = ExpectedException.none();

	  protected internal ProcessInstance processInstance;
	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal ExternalTaskService externalTaskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = engineRule.RuntimeService;
		historyService = engineRule.HistoryService;
		externalTaskService = engineRule.ExternalTaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTimestampAsc()
	  public virtual void testQuerySortingByTimestampAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByTimestamp().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskByTimestamp());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTimestampDsc()
	  public virtual void testQuerySortingByTimestampDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByTimestamp().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskByTimestamp()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTaskIdAsc()
	  public virtual void testQuerySortingByTaskIdAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByExternalTaskId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByExternalTaskId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTaskIdDsc()
	  public virtual void testQuerySortingByTaskIdDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByExternalTaskId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByExternalTaskId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByRetriesAsc()
	  public virtual void testQuerySortingByRetriesAsc()
	  {

		// given
		int taskCount = 10;
		IList<ExternalTask> list = startProcesses(taskCount);
		reportExternalTaskFailure(list);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.failureLog().orderByRetries().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByRetries());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByRetriesDsc()
	  public virtual void testQuerySortingByRetriesDsc()
	  {

		// given
		int taskCount = 10;
		IList<ExternalTask> list = startProcesses(taskCount);
		reportExternalTaskFailure(list);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.failureLog().orderByRetries().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByRetries()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByPriorityAsc()
	  public virtual void testQuerySortingByPriorityAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByPriority().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByPriority());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByPriorityDsc()
	  public virtual void testQuerySortingByPriorityDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByPriority().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByPriority()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTopicNameAsc()
	  public virtual void testQuerySortingByTopicNameAsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByTopicName(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByTopicName().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByTopicName());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByTopicNameDsc()
	  public virtual void testQuerySortingByTopicNameDsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByTopicName(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByTopicName().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByTopicName()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByWorkerIdAsc()
	  public virtual void testQuerySortingByWorkerIdAsc()
	  {

		// given
		int taskCount = 10;
		IList<ExternalTask> list = startProcesses(taskCount);
		completeExternalTasksWithWorkers(list);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.successLog().orderByWorkerId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByWorkerId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByWorkerIdDsc()
	  public virtual void testQuerySortingByWorkerIdDsc()
	  {

		// given
		int taskCount = 10;
		IList<ExternalTask> list = startProcesses(taskCount);
		completeExternalTasksWithWorkers(list);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.successLog().orderByWorkerId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByWorkerId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByActivityIdAsc()
	  public virtual void testQuerySortingByActivityIdAsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByActivityId(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByActivityId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByActivityId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByActivityIdDsc()
	  public virtual void testQuerySortingByActivityIdDsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByActivityId(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByActivityId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByActivityId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByActivityInstanceIdAsc()
	  public virtual void testQuerySortingByActivityInstanceIdAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByActivityInstanceId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByActivityInstanceId());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByActivityInstanceIdDsc()
	  public virtual void testQuerySortingByActivityInstanceIdDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByActivityInstanceId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByActivityInstanceId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByExecutionIdAsc()
	  public virtual void testQuerySortingByExecutionIdAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByExecutionId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByExecutionId());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByExecutionIdDsc()
	  public virtual void testQuerySortingByExecutionIdDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByExecutionId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByExecutionId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessInstanceIdAsc()
	  public virtual void testQuerySortingByProcessInstanceIdAsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessInstanceId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByProcessInstanceId());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessInstanceIdDsc()
	  public virtual void testQuerySortingByProcessInstanceIdDsc()
	  {

		// given
		int taskCount = 10;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessInstanceId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByProcessInstanceId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionIdAsc()
	  public virtual void testQuerySortingByProcessDefinitionIdAsc()
	  {

		// given
		int taskCount = 8;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessDefinitionId().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByProcessDefinitionId());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionIdDsc()
	  public virtual void testQuerySortingByProcessDefinitionIdDsc()
	  {

		// given
		int taskCount = 8;
		startProcesses(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessDefinitionId().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByProcessDefinitionId()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionKeyAsc()
	  public virtual void testQuerySortingByProcessDefinitionKeyAsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByProcessDefinitionKey(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessDefinitionKey().asc();

		// then
		verifyQueryWithOrdering(query, taskCount, historicExternalTaskLogByProcessDefinitionKey(engineRule.ProcessEngine));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySortingByProcessDefinitionKeyDsc()
	  public virtual void testQuerySortingByProcessDefinitionKeyDsc()
	  {

		// given
		int taskCount = 10;
		startProcessesByProcessDefinitionKey(taskCount);

		// when
		HistoricExternalTaskLogQuery query = historyService.createHistoricExternalTaskLogQuery();
		query.orderByProcessDefinitionKey().desc();

		// then
		verifyQueryWithOrdering(query, taskCount, inverted(historicExternalTaskLogByProcessDefinitionKey(engineRule.ProcessEngine)));
	  }

	  // helper ------------------------------------

	  protected internal virtual void completeExternalTasksWithWorkers(IList<ExternalTask> taskLIst)
	  {
		for (int? i = 0; i < taskLIst.Count; i++)
		{
		  completeExternalTaskWithWorker(taskLIst[i].Id, i.ToString());
		}
	  }

	  protected internal virtual void completeExternalTaskWithWorker(string externalTaskId, string workerId)
	  {
		completeExternalTask(externalTaskId, DEFAULT_TOPIC, workerId, false);

	  }

	  protected internal virtual void completeExternalTask(string externalTaskId, string topic, string workerId, bool usePriority)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, workerId, usePriority).topic(topic, LOCK_DURATION).execute();
		externalTaskService.complete(externalTaskId, workerId);
		// unlock the remaining tasks
		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  if (!lockedExternalTask.Id.Equals(externalTaskId))
		  {
			externalTaskService.unlock(lockedExternalTask.Id);
		  }
		}
	  }

	  protected internal virtual void reportExternalTaskFailure(IList<ExternalTask> taskLIst)
	  {
		for (int? i = 0; i < taskLIst.Count; i++)
		{
		  reportExternalTaskFailure(taskLIst[i].Id, DEFAULT_TOPIC, WORKER_ID, i + 1, false, "foo");
		}
	  }

	  protected internal virtual void reportExternalTaskFailure(string externalTaskId, string topic, string workerId, int? retries, bool usePriority, string errorMessage)
	  {
		IList<LockedExternalTask> list = externalTaskService.fetchAndLock(100, workerId, usePriority).topic(topic, LOCK_DURATION).execute();
		externalTaskService.handleFailure(externalTaskId, workerId, errorMessage, retries.Value, 0L);

		foreach (LockedExternalTask lockedExternalTask in list)
		{
		  externalTaskService.unlock(lockedExternalTask.Id);
		}
	  }

	  protected internal virtual IList<ExternalTask> startProcesses(int count)
	  {
		IList<ExternalTask> list = new LinkedList<ExternalTask>();
		for (int ithPrio = 0; ithPrio < count; ithPrio++)
		{
		  list.Add(startExternalTaskProcessGivenPriority(ithPrio));
		  ensureEnoughTimePassedByForTimestampOrdering();
		}
		return list;
	  }

	  protected internal virtual IList<ExternalTask> startProcessesByTopicName(int count)
	  {
		IList<ExternalTask> list = new LinkedList<ExternalTask>();
		for (int? ithTopic = 0; ithTopic.Value < count; ithTopic++)
		{
		  list.Add(startExternalTaskProcessGivenTopicName(ithTopic.ToString()));
		}
		return list;
	  }

	  protected internal virtual IList<ExternalTask> startProcessesByActivityId(int count)
	  {
		IList<ExternalTask> list = new LinkedList<ExternalTask>();
		for (int? ithTopic = 0; ithTopic.Value < count; ithTopic++)
		{
		  list.Add(startExternalTaskProcessGivenActivityId("Activity" + ithTopic.ToString()));
		}
		return list;
	  }

	  protected internal virtual IList<ExternalTask> startProcessesByProcessDefinitionKey(int count)
	  {
		IList<ExternalTask> list = new LinkedList<ExternalTask>();
		for (int? ithTopic = 0; ithTopic.Value < count; ithTopic++)
		{
		  list.Add(startExternalTaskProcessGivenProcessDefinitionKey("ProcessKey" + ithTopic.ToString()));
		}
		return list;
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenTopicName(string topicName)
	  {
		BpmnModelInstance processModelWithCustomTopic = createDefaultExternalTaskModel().topic(topicName).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomTopic);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenActivityId(string activityId)
	  {
		BpmnModelInstance processModelWithCustomActivityId = createDefaultExternalTaskModel().externalTaskName(activityId).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomActivityId);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenProcessDefinitionKey(string processDefinitionKey)
	  {
		BpmnModelInstance processModelWithCustomKey = createDefaultExternalTaskModel().processKey(processDefinitionKey).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomKey);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual ExternalTask startExternalTaskProcessGivenPriority(int priority)
	  {
		BpmnModelInstance processModelWithCustomPriority = createDefaultExternalTaskModel().priority(priority).build();
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(processModelWithCustomPriority);
		ProcessInstance pi = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		return externalTaskService.createExternalTaskQuery().processInstanceId(pi.Id).singleResult();
	  }

	  protected internal virtual void verifyQueryWithOrdering(HistoricExternalTaskLogQuery query, int countExpected, TestOrderingUtil.NullTolerantComparator<HistoricExternalTaskLog> expectedOrdering)
	  {
		assertThat(countExpected, @is(query.list().size()));
		assertThat((long) countExpected, @is(query.count()));
		verifySorting(query.list(), expectedOrdering);
	  }

	  protected internal virtual void ensureEnoughTimePassedByForTimestampOrdering()
	  {
		long timeToAddInSeconds = 5 * 1000L;
		DateTime nowPlus5Seconds = new DateTime(ClockUtil.CurrentTime.Ticks + timeToAddInSeconds);
		ClockUtil.CurrentTime = nowPlus5Seconds;
	  }

	}

}