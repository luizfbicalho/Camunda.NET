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
namespace org.camunda.bpm.qa.rolling.update.eventSubProcess
{
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithEventSubProcessScenario")]
	public class CompleteProcessWithEventSubProcessTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteProcessWithEventSubProcess()
		[ScenarioUnderTest("init.1")]
		public virtual void testCompleteProcessWithEventSubProcess()
		{
		//given process within event sub process
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);
		Job job = rule.jobQuery().singleResult();
		Assert.assertNotNull(job);

		//when job is executed
		rule.ManagementService.executeJob(job.Id);

		//then delegate fails and event sub process is called
		Task task = rule.TaskService.createTaskQuery().processInstanceId(oldInstance.Id).taskName("TaskInEventSubProcess").singleResult();
		Assert.assertNotNull(task);
		rule.TaskService.complete(task.Id);
		rule.assertScenarioEnded();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.error.1") public void testCompleteProcessWithInEventSubProcess()
	  [ScenarioUnderTest("init.error.1")]
	  public virtual void testCompleteProcessWithInEventSubProcess()
	  {
		//given process within event sub process
		ProcessInstance oldInstance = rule.processInstance();
		Task task = rule.TaskService.createTaskQuery().processInstanceId(oldInstance.Id).taskName("TaskInEventSubProcess").singleResult();
		Assert.assertNotNull(task);

		//when task is completed
		rule.TaskService.complete(task.Id);

		//process instance is ended
		rule.assertScenarioEnded();
	  }
	}

}