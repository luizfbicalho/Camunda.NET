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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.gson
{
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
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
	[ScenarioUnderTest("ProcessInstanceModificationScenario"), Origin("7.11.0")]
	public class ProcessInstanceModificationTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initProcessInstanceModification.1") @Test public void testModificationBatch()
	  [ScenarioUnderTest("initProcessInstanceModification.1")]
	  public virtual void testModificationBatch()
	  {
		IList<Execution> executions = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("oneTaskProcessInstanceModification_710").processInstanceBusinessKey("ProcessInstanceModificationScenario").active().list();

		// assume
		assertThat(executions.Count, @is(3));

		IList<Batch> batches = engineRule.ManagementService.createBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_PROCESS_INSTANCE_MODIFICATION).list();

		Batch processInstanceModification = findBatchByTotalJobs(1, batches);

		string jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(processInstanceModification.SeedJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);

		Job job = engineRule.ManagementService.createJobQuery().jobDefinitionId(processInstanceModification.BatchJobDefinitionId).singleResult();

		// when
		engineRule.ManagementService.executeJob(job.Id);

		executions = engineRule.RuntimeService.createExecutionQuery().processDefinitionKey("oneTaskProcessInstanceModification_710").processInstanceBusinessKey("ProcessInstanceModificationScenario").active().list();

		// then
		assertThat(executions.Count, @is(0));
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

	}
}