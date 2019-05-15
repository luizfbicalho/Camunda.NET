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
namespace org.camunda.bpm.engine.test.api.mgmt
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
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
	public class ManagementServiceAsyncOperationsTest : AbstractAsyncOperationsTest
	{
		private bool InstanceFieldsInitialized = false;

		public ManagementServiceAsyncOperationsTest()
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

	  protected internal const int RETRIES = 5;
	  protected internal const string TEST_PROCESS = "exceptionInJobExecution";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

	  protected internal IList<string> processInstanceIds;
	  protected internal IList<string> ids;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public override void initServices()
	  {
		base.initServices();
		prepareData();
	  }

	  public virtual void prepareData()
	  {
		testRule.deploy("org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml");
		processInstanceIds = startTestProcesses(2);
		ids = AllJobIds;
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

	  protected internal virtual IList<string> AllJobIds
	  {
		  get
		  {
			List<string> result = new List<string>();
			foreach (Job job in managementService.createJobQuery().list())
			{
			  if (!string.ReferenceEquals(job.ProcessInstanceId, null))
			  {
				result.Add(job.Id);
			  }
			}
			return result;
		  }
	  }

	  protected internal override IList<string> startTestProcesses(int numberOfProcesses)
	  {
		List<string> ids = new List<string>();

		for (int i = 0; i < numberOfProcesses; i++)
		{
		  ids.Add(runtimeService.startProcessInstanceByKey(TEST_PROCESS).ProcessInstanceId);
		}

		return ids;
	  }

	  protected internal virtual void assertRetries(IList<string> allJobIds, int i)
	  {
		foreach (string id in allJobIds)
		{
		  Assert.assertThat(managementService.createJobQuery().jobId(id).singleResult().Retries, @is(i));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithJobList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithJobList()
	  {
		//when
		Batch batch = managementService.setJobRetriesAsync(ids, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(ids, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithProcessList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithProcessList()
	  {
		//when
		Batch batch = managementService.setJobRetriesAsync(processInstanceIds, (ProcessInstanceQuery) null, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(ids, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithEmptyJobList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithEmptyJobList()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		managementService.setJobRetriesAsync(new List<string>(), RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithEmptyProcessList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithEmptyProcessList()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		managementService.setJobRetriesAsync(new List<string>(), (ProcessInstanceQuery) null, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNonExistingJobID() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNonExistingJobID()
	  {
		//given
		ids.Add("aFake");

		//when
		Batch batch = managementService.setJobRetriesAsync(ids, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		//then
		assertThat(exceptions.Count, @is(1));
		assertRetries(AllJobIds, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNonExistingProcessID() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNonExistingProcessID()
	  {
		//given
		processInstanceIds.Add("aFake");

		//when
		Batch batch = managementService.setJobRetriesAsync(processInstanceIds, (ProcessInstanceQuery) null, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		//then
		assertThat(exceptions.Count, @is(0));
		assertRetries(AllJobIds, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithJobQueryAndList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithJobQueryAndList()
	  {
		//given
		IList<string> extraPi = startTestProcesses(1);
		JobQuery query = managementService.createJobQuery().processInstanceId(extraPi[0]);

		//when
		Batch batch = managementService.setJobRetriesAsync(ids, query, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(AllJobIds, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithProcessQueryAndList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithProcessQueryAndList()
	  {
		//given
		IList<string> extraPi = startTestProcesses(1);
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processInstanceId(extraPi[0]);

		//when
		Batch batch = managementService.setJobRetriesAsync(processInstanceIds, query, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(AllJobIds, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithJobQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithJobQuery()
	  {
		//given
		JobQuery query = managementService.createJobQuery();

		//when
		Batch batch = managementService.setJobRetriesAsync(query, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(ids, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithProcessQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithProcessQuery()
	  {
		//given
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery();

		//when
		Batch batch = managementService.setJobRetriesAsync(null, query, RETRIES);
		executeSeedJob(batch);
		IList<Exception> exceptions = executeBatchJobs(batch);

		// then
		assertThat(exceptions.Count, @is(0));
		assertRetries(ids, RETRIES);
		assertHistoricBatchExists(testRule);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithEmptyJobQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithEmptyJobQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//given
		JobQuery query = managementService.createJobQuery().suspended();

		//when
		managementService.setJobRetriesAsync(query, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithEmptyProcessQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithEmptyProcessQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//given
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().suspended();

		//when
		managementService.setJobRetriesAsync(null, query, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNonExistingIDAsJobQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNonExistingIDAsJobQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//given
		JobQuery query = managementService.createJobQuery().jobId(ids[0]).jobId("aFake");

		//when
		managementService.setJobRetriesAsync(query, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNonExistingIDAsProcessQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNonExistingIDAsProcessQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//given
		ProcessInstanceQuery query = runtimeService.createProcessInstanceQuery().processInstanceId("aFake");

		//when
		managementService.setJobRetriesAsync(null, query, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNullJobList() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNullJobList()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		managementService.setJobRetriesAsync((List<object>) null, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNullJobQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNullJobQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		managementService.setJobRetriesAsync((JobQuery) null, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNullProcessQuery() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNullProcessQuery()
	  {
		//expect
		thrown.expect(typeof(ProcessEngineException));

		//when
		managementService.setJobRetriesAsync(null, (ProcessInstanceQuery) null, RETRIES);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSetJobsRetryAsyncWithNegativeRetries() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testSetJobsRetryAsyncWithNegativeRetries()
	  {
		//given
		JobQuery query = managementService.createJobQuery();

		//when
		thrown.expect(typeof(ProcessEngineException));
		managementService.setJobRetriesAsync(query, -1);
	  }
	}

}