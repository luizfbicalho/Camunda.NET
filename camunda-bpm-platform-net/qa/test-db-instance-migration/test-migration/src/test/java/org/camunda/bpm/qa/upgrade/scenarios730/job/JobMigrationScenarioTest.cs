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
namespace org.camunda.bpm.qa.upgrade.scenarios730.job
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// This actually tests migration from 7.0 jobs (where there was no suspension state)
	/// to 7.4 (where suspension state is a not-null column).
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	[ScenarioUnderTest("JobMigrationScenario"), Origin("7.3.0")]
	public class JobMigrationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("createJob.1") public void testSuspensionState()
	  [ScenarioUnderTest("createJob.1")]
	  public virtual void testSuspensionState()
	  {
		// given
		Job job = rule.ManagementService.createJobQuery().jobId(rule.BuisnessKey).singleResult();

		// then
		Assert.assertNotNull(job);
		Assert.assertFalse(job.Suspended);
	  }
	}

}