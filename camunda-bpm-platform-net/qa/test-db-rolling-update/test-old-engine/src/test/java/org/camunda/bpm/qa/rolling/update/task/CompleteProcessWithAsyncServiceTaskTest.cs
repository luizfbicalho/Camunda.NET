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
namespace org.camunda.bpm.qa.rolling.update.task
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// This test ensures that the old engine can complete an
	/// existing process with service task on the new schema.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithAsyncServiceTaskScenario")]
	public class CompleteProcessWithAsyncServiceTaskTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteWithAsyncServiceTask()
		[ScenarioUnderTest("init.1")]
		public virtual void testCompleteWithAsyncServiceTask()
		{
		//given process with async service task
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);
		Job job = rule.jobQuery().singleResult();
		Assert.assertNotNull(job);

		//when job is executed
		rule.ManagementService.executeJob(job.Id);

		//then existing job will executed and process instance ends
		rule.assertScenarioEnded();
		}

	}

}