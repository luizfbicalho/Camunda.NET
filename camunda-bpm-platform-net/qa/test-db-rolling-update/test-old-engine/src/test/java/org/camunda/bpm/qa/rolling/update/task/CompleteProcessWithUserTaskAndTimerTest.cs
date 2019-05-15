using System;

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
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;


	/// <summary>
	/// This test ensures that the old engine can complete an
	/// existing process with user task and timer on the new schema.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithUserTaskAndTimerScenario")]
	public class CompleteProcessWithUserTaskAndTimerTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteProcessWithUserTaskAndTimer() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		[ScenarioUnderTest("init.1")]
		public virtual void testCompleteProcessWithUserTaskAndTimer()
		{
		//given a process instance with user task and timer boundary event
		Job job = rule.jobQuery().singleResult();
		Assert.assertNotNull(job);
		//job is not available since timer is set to 2 mintues in the future
		Assert.assertFalse(!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime > job.Duedate));

		//when time is incremented by five minutes
		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 60 * 1000 * 5);

		//then job is available and timer should executed and process instance ends
		job = rule.jobQuery().singleResult();
		Assert.assertTrue(!job.Suspended && job.Retries > 0 && (job.Duedate == null || ClockUtil.CurrentTime > job.Duedate));
		rule.ManagementService.executeJob(job.Id);
		rule.assertScenarioEnded();
		}

	}

}