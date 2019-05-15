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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.gson.batch
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[ScenarioUnderTest("ModificationBatchScenario"), Origin("7.11.0")]
	public class ModificationBatchTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initModificationBatch.1") @Test public void testProcessInstanceModification()
	  [ScenarioUnderTest("initModificationBatch.1")]
	  public virtual void testProcessInstanceModification()
	  {
		IList<Execution> executions = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("oneTaskProcessModification_710").processInstanceBusinessKey("ModificationBatchScenario").activityId("userTask1").list();

		// assume
		assertThat(executions.Count, @is(10));

		IList<Batch> batches = engineRule.ManagementService.createBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION).list();

		Batch modificationBatch = findBatchByTotalJobs(10, batches);

		string jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(modificationBatch.SeedJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);

		IList<Job> jobs = engineRule.ManagementService.createJobQuery().jobDefinitionId(modificationBatch.BatchJobDefinitionId).list();

		// when
		foreach (Job job in jobs)
		{
		  engineRule.ManagementService.executeJob(job.Id);
		}

		IList<Execution> executionsInUserTaskOne = getExecutionsByActivityId("userTask1");

		IList<Execution> executionsInUserTaskTwo = getExecutionsByActivityId("userTask2");

		IList<Execution> executionsInUserTaskThree = getExecutionsByActivityId("userTask3");

		IList<Execution> executionsInUserTaskFour = getExecutionsByActivityId("userTask4");

		IList<ProcessInstance> processInstances = engineRule.RuntimeService.createProcessInstanceQuery().processDefinitionKey("oneTaskProcessModification_710").processInstanceBusinessKey("ModificationBatchScenario").list();

		IList<HistoricActivityInstance> canceledActivityInstances = engineRule.HistoryService.createHistoricActivityInstanceQuery().processInstanceId(processInstances[0].Id).canceled().list();

		// then
		assertThat(executionsInUserTaskOne.Count, @is(20));
		assertThat(executionsInUserTaskTwo.Count, @is(10));
		assertThat(executionsInUserTaskThree.Count, @is(10));

		assertThat(executionsInUserTaskFour.Count, @is(0));
		assertThat(canceledActivityInstances.Count, @is(1));
	  }

	  protected internal virtual Batch findBatchByTotalJobs(int totalJobsCount, IList<Batch> batches)
	  {
		foreach (Batch batch in batches)
		{
		  if (batch.TotalJobs == totalJobsCount)
		  {
			return batch;
		  }
		}

		return null;
	  }

	  protected internal virtual IList<Execution> getExecutionsByActivityId(string activityId)
	  {
		return engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("oneTaskProcessModification_710").processInstanceBusinessKey("ModificationBatchScenario").activityId(activityId).list();
	  }

	}
}