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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
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
	[ScenarioUnderTest("SetExternalTaskRetriesScenario"), Origin("7.11.0")]
	public class SetExternalTaskRetriesBatchTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initSetExternalTaskRetriesBatch.1") @Test public void testSetExternalTaskRetriesBatch()
	  [ScenarioUnderTest("initSetExternalTaskRetriesBatch.1")]
	  public virtual void testSetExternalTaskRetriesBatch()
	  {
		IList<ExternalTask> externalTasks = engineRule.ExternalTaskService.createExternalTaskQuery().topicName("topic710").list();

		// assume
		assertThat(externalTasks.Count, @is(10));

		Batch batch = engineRule.ManagementService.createBatchQuery().type(org.camunda.bpm.engine.batch.Batch_Fields.TYPE_SET_EXTERNAL_TASK_RETRIES).singleResult();

		string jobId = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.SeedJobDefinitionId).singleResult().Id;

		engineRule.ManagementService.executeJob(jobId);

		IList<Job> jobs = engineRule.ManagementService.createJobQuery().jobDefinitionId(batch.BatchJobDefinitionId).list();

		// when
		foreach (Job job in jobs)
		{
		  engineRule.ManagementService.executeJob(job.Id);
		}

		externalTasks = engineRule.ExternalTaskService.createExternalTaskQuery().topicName("topic710").list();

		// then
		assertThat(externalTasks.Count, @is(10));

		foreach (ExternalTask externalTask in externalTasks)
		{
		  assertThat(externalTask.Retries, @is(22));
		}
	  }

	}
}