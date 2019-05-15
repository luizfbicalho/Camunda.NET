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
namespace org.camunda.bpm.engine.test.api
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Askar Akhmerov
	/// </summary>
	public abstract class AbstractAsyncOperationsTest
	{
		private bool InstanceFieldsInitialized = false;

		public AbstractAsyncOperationsTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
		}


	  public const string ONE_TASK_PROCESS = "oneTaskProcess";
	  public const string TESTING_INSTANCE_DELETE = "testing instance delete";

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;
	  protected internal HistoryService historyService;

	  public ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  public ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = engineRule.RuntimeService;
		managementService = engineRule.ManagementService;
		historyService = engineRule.HistoryService;
	  }

	  protected internal virtual void executeSeedJob(Batch batch)
	  {
		string seedJobDefinitionId = batch.SeedJobDefinitionId;
		Job seedJob = managementService.createJobQuery().jobDefinitionId(seedJobDefinitionId).singleResult();
		assertNotNull(seedJob);
		managementService.executeJob(seedJob.Id);
	  }

	  /// <summary>
	  /// Execute all batch jobs of batch once and collect exceptions during job execution.
	  /// </summary>
	  /// <param name="batch"> the batch for which the batch jobs should be executed </param>
	  /// <returns> the catched exceptions of the batch job executions, is empty if non where thrown </returns>
	  protected internal virtual IList<Exception> executeBatchJobs(Batch batch)
	  {
		string batchJobDefinitionId = batch.BatchJobDefinitionId;
		IList<Job> batchJobs = managementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).list();
		assertFalse(batchJobs.Count == 0);

		IList<Exception> catchedExceptions = new List<Exception>();

		foreach (Job batchJob in batchJobs)
		{
		  try
		  {
			managementService.executeJob(batchJob.Id);
		  }
		  catch (Exception e)
		  {
			catchedExceptions.Add(e);
		  }
		}

		return catchedExceptions;
	  }

	  protected internal virtual IList<string> startTestProcesses(int numberOfProcesses)
	  {
		List<string> ids = new List<string>();

		for (int i = 0; i < numberOfProcesses; i++)
		{
		  ids.Add(runtimeService.startProcessInstanceByKey(ONE_TASK_PROCESS).ProcessInstanceId);
		}

		return ids;
	  }

	  protected internal virtual void assertHistoricTaskDeletionPresent(IList<string> processIds, string deleteReason, ProcessEngineTestRule testRule)
	  {
		if (!testRule.HistoryLevelNone)
		{

		  foreach (string processId in processIds)
		  {
			HistoricTaskInstance historicTaskInstance = historyService.createHistoricTaskInstanceQuery().processInstanceId(processId).singleResult();

			assertThat(historicTaskInstance.DeleteReason, @is(deleteReason));
		  }
		}
	  }

	  protected internal virtual void assertHistoricBatchExists(ProcessEngineTestRule testRule)
	  {
		if (testRule.HistoryLevelFull)
		{
		  assertThat(historyService.createHistoricBatchQuery().count(), @is(1L));
		}
	  }

	}

}