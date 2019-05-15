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
	using TaskService = org.camunda.bpm.engine.TaskService;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Assert = org.junit.Assert;
	using Test = org.junit.Test;

	/// <summary>
	/// This test ensures that the old engine can complete an
	/// existing process with user task on the new schema.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithUserTaskScenario")]
	public class CompleteProcessWithUserTaskTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteProcessWithUserTask()
		[ScenarioUnderTest("init.1")]
		public virtual void testCompleteProcessWithUserTask()
		{
		//given an already started process instance
		ProcessInstance oldInstance = rule.processInstance();
		Assert.assertNotNull(oldInstance);

		//which waits on an user task
		TaskService taskService = rule.TaskService;
		Task userTask = taskService.createTaskQuery().processInstanceId(oldInstance.Id).singleResult();
		Assert.assertNotNull(userTask);

		//when completing the user task
		taskService.complete(userTask.Id);

		//then there exists no more tasks
		//and the process instance is also completed
		Assert.assertEquals(0, rule.taskQuery().count());
		rule.assertScenarioEnded();
		}

	}

}