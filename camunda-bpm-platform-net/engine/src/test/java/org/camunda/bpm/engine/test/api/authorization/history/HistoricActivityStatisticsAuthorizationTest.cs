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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;

	using HistoricActivityStatistics = org.camunda.bpm.engine.history.HistoricActivityStatistics;
	using HistoricActivityStatisticsQuery = org.camunda.bpm.engine.history.HistoricActivityStatisticsQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoricActivityStatisticsAuthorizationTest : AuthorizationTest
	{

	  protected internal const string PROCESS_KEY = "oneTaskProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // historic activity statistics query //////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		// then
		verifyQueryResults(query, 1);
		verifyStatisticsResult(query.singleResult(), 3, 0, 0, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		// then
		verifyQueryResults(query, 1);
		verifyStatisticsResult(query.singleResult(), 3, 0, 0, 0);
	  }

	  public virtual void testQueryMultiple()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId);

		// then
		verifyQueryResults(query, 1);
		verifyStatisticsResult(query.singleResult(), 3, 0, 0, 0);
	  }

	  // historic activity statistics query (including finished) //////////////////////////////////

	  public virtual void testQueryIncludingFinishedWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryIncludingFinishedWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished();

		// then
		verifyQueryResults(query, 3);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
		verifyStatisticsResult(start, 0, 3, 0, 0);

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 1, 0, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 1, 0, 0);
	  }

	  public virtual void testQueryIncludingFinishedWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished();

		// then
		verifyQueryResults(query, 3);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
		verifyStatisticsResult(start, 0, 3, 0, 0);

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 1, 0, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 1, 0, 0);
	  }

	  // historic activity statistics query (including canceled) //////////////////////////////////

	  public virtual void testQueryIncludingCanceledWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryIncludingCanceledWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled();

		// then
		verifyQueryResults(query, 1);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 0, 1, 0);
	  }

	  public virtual void testQueryIncludingCanceledWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCanceled();

		// then
		verifyQueryResults(query, 1);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 0, 1, 0);
	  }

	  // historic activity statistics query (including complete scope) //////////////////////////////////

	  public virtual void testQueryIncludingCompleteScopeWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryIncludingCompleteScopeWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope();

		// then
		verifyQueryResults(query, 2);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 0, 0, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 0, 0, 1);
	  }

	  public virtual void testQueryIncludingCompleteScopeWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeCompleteScope();

		// then
		verifyQueryResults(query, 2);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 2, 0, 0, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 0, 0, 1);
	  }

	  // historic activity statistics query (including all) //////////////////////////////////

	  public virtual void testQueryIncludingAllWithoutAuthorization()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().includeCanceled().includeCompleteScope();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryIncludingAllWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().includeCanceled().includeCompleteScope();

		// then
		verifyQueryResults(query, 3);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
		verifyStatisticsResult(start, 0, 3, 0, 0);

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 1, 2, 1, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 1, 0, 1);
	  }

	  public virtual void testQueryIncludingAllWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processDefinitionId = selectProcessDefinitionByKey(PROCESS_KEY).Id;

		startProcessInstanceByKey(PROCESS_KEY);
		startProcessInstanceByKey(PROCESS_KEY);
		string processInstanceId = startProcessInstanceByKey(PROCESS_KEY).Id;

		disableAuthorization();
		runtimeService.deleteProcessInstance(processInstanceId, null);
		enableAuthorization();

		string taskId = selectAnyTask().Id;
		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricActivityStatisticsQuery query = historyService.createHistoricActivityStatisticsQuery(processDefinitionId).includeFinished().includeCanceled().includeCompleteScope();

		// then
		verifyQueryResults(query, 3);
		IList<HistoricActivityStatistics> statistics = query.list();

		HistoricActivityStatistics start = getStatisticsByKey(statistics, "theStart");
		verifyStatisticsResult(start, 0, 3, 0, 0);

		HistoricActivityStatistics task = getStatisticsByKey(statistics, "theTask");
		verifyStatisticsResult(task, 1, 2, 1, 0);

		HistoricActivityStatistics end = getStatisticsByKey(statistics, "theEnd");
		verifyStatisticsResult(end, 0, 1, 0, 1);
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricActivityStatisticsQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void verifyStatisticsResult(HistoricActivityStatistics statistics, int instances, int finished, int canceled, int completeScope)
	  {
		assertEquals("Instances", instances, statistics.Instances);
		assertEquals("Finished", finished, statistics.Finished);
		assertEquals("Canceled", canceled, statistics.Canceled);
		assertEquals("Complete Scope", completeScope, statistics.CompleteScope);
	  }

	  protected internal virtual HistoricActivityStatistics getStatisticsByKey(IList<HistoricActivityStatistics> statistics, string key)
	  {
		foreach (HistoricActivityStatistics result in statistics)
		{
		  if (key.Equals(result.Id))
		  {
			return result;
		  }
		}
		fail("No statistics found for key '" + key + "'.");
		return null;
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