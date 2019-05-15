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
namespace org.camunda.bpm.engine.test.api.history
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using HistoricProcessInstanceQuery = org.camunda.bpm.engine.history.HistoricProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class HistoryServiceAsyncOperationsTest : AbstractAsyncOperationsTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryServiceAsyncOperationsTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal const string TEST_REASON = "test reason";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal TaskService taskService;
	  protected internal IList<string> historicProcessInstances;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public override void initServices()
	  {
		base.initServices();
		taskService = engineRule.TaskService;
		prepareData();
	  }

	  public virtual void prepareData()
	  {
		testRule.deploy("org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml");
		startTestProcesses(2);

		foreach (Task activeTask in taskService.createTaskQuery().list())
		{
		  taskService.complete(activeTask.Id);
		}

		historicProcessInstances = new List<string>();
		foreach (HistoricProcessInstance pi in historyService.createHistoricProcessInstanceQuery().list())
		{
		  historicProcessInstances.Add(pi.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void cleanBatch()
	  public virtual void cleanBatch()
	  {
		Batch batch = managementService.createBatchQuery().singleResult();
		if (batch != null)
		{
		  managementService.deleteBatch(batch.Id, true);
		}

		HistoricBatch historicBatch = historyService.createHistoricBatchQuery().singleResult();
		if (historicBatch != null)
		{
		  historyService.deleteHistoricBatch(historicBatch.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithList()
	  {
		//when
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(historicProcessInstances, TEST_REASON);

		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertNoHistoryForTasks();
		assertHistoricBatchExists(testRule);
		assertAllHistoricProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithEmptyList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithEmptyList()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		historyService.deleteHistoricProcessInstancesAsync(new List<string>(), TEST_REASON);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithFake() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithFake()
	  {
		//given
		List<string> processInstanceIds = new List<string>();
		processInstanceIds.Add(historicProcessInstances[0]);
		processInstanceIds.Add("aFakeId");

		//when
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(processInstanceIds, TEST_REASON);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		//then
		assertThat(exceptions.Count, @is(0));
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithQueryAndList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithQueryAndList()
	  {
		//given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processInstanceId(historicProcessInstances[0]);
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(historicProcessInstances.subList(1, historicProcessInstances.Count), query, TEST_REASON);
		executeSeedJob(batch);

		//when
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertNoHistoryForTasks();
		assertHistoricBatchExists(testRule);
		assertAllHistoricProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithQuery()
	  {
		//given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processInstanceIds(new HashSet<string>(historicProcessInstances));
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(query, TEST_REASON);
		executeSeedJob(batch);

		//when
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertNoHistoryForTasks();
		assertHistoricBatchExists(testRule);
		assertAllHistoricProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithEmptyQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithEmptyQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));
		//given
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().unfinished();
		//when
		historyService.deleteHistoricProcessInstancesAsync(query, TEST_REASON);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithNonExistingIDAsQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithNonExistingIDAsQuery()
	  {
		//given
		List<string> processInstanceIds = new List<string>();
		processInstanceIds.Add(historicProcessInstances[0]);
		processInstanceIds.Add("aFakeId");
		HistoricProcessInstanceQuery query = historyService.createHistoricProcessInstanceQuery().processInstanceIds(new HashSet<object>(processInstanceIds));

		//when
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(query, TEST_REASON);
		executeSeedJob(batch);
		executeBatchJobs(batch);

		//then
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithoutDeleteReason() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithoutDeleteReason()
	  {
		//when
		Batch batch = historyService.deleteHistoricProcessInstancesAsync(historicProcessInstances, null);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		//then
		assertThat(exceptions.Count, @is(0));
		assertNoHistoryForTasks();
		assertHistoricBatchExists(testRule);
		assertAllHistoricProcessInstancesAreDeleted();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithNullList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithNullList()
	  {
		thrown.expect(typeof(ProcessEngineException));
		historyService.deleteHistoricProcessInstancesAsync((System.Collections.IList) null, TEST_REASON);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteHistoryProcessInstancesAsyncWithNullQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testDeleteHistoryProcessInstancesAsyncWithNullQuery()
	  {
		thrown.expect(typeof(ProcessEngineException));
		historyService.deleteHistoricProcessInstancesAsync((HistoricProcessInstanceQuery) null, TEST_REASON);
	  }

	  protected internal virtual void assertNoHistoryForTasks()
	  {
		if (!testRule.HistoryLevelNone)
		{
		  Assert.assertThat(historyService.createHistoricTaskInstanceQuery().count(), CoreMatchers.@is(0L));
		}
	  }

	  protected internal virtual void assertAllHistoricProcessInstancesAreDeleted()
	  {
		assertThat(historyService.createHistoricProcessInstanceQuery().count(), @is(0L));
	  }

	}

}