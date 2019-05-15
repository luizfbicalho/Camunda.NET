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
namespace org.camunda.bpm.qa.rolling.update.multiInstance
{
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using ScenarioUnderTest = org.camunda.bpm.qa.upgrade.ScenarioUnderTest;
	using Test = org.junit.Test;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	/// 
	/// <summary>
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	[ScenarioUnderTest("ProcessWithMultiInstanceCallActivityScenario")]
	public class CompleteProcessWithMultiInstanceCallActivityTest : AbstractRollingUpdateTestCase
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompleteProcessWithCallActivity()
		[ScenarioUnderTest("init.1")]
		public virtual void testCompleteProcessWithCallActivity()
		{
		//given process with user task before multi-instance call activity
		ProcessInstance processInstance = rule.processInstance();
		TaskQuery taskQuery = rule.TaskService.createTaskQuery().processInstanceId(processInstance.Id);
		Task taskBeforeSubProcess = taskQuery.taskName("Task before multi-instance").singleResult();
		assertNotNull(taskBeforeSubProcess);

		//when the task before is complete the process leads to calling the multi-instance subprocess
		rule.TaskService.complete(taskBeforeSubProcess.Id);

		Task taskAfterSubProcess = taskQuery.taskName("Task after multi-instance").singleResult();
		assertNotNull(taskAfterSubProcess);

		//after completing the after task the process instance ends
		rule.TaskService.complete(taskAfterSubProcess.Id);
		rule.assertScenarioEnded();
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.complete.one.1") public void testCompleteProcessWithCallActivityAndOneCompletedTask()
	  [ScenarioUnderTest("init.complete.one.1")]
	  public virtual void testCompleteProcessWithCallActivityAndOneCompletedTask()
	  {
		//given process after multi-instance callactivity
		ProcessInstance processInstance = rule.processInstance();
		TaskQuery taskQuery = rule.TaskService.createTaskQuery().processInstanceId(processInstance.Id);
		Task taskAfterSubProcess = taskQuery.taskName("Task after multi-instance").singleResult();
		assertNotNull(taskAfterSubProcess);

		// Completing this task end the process instance
		rule.TaskService.complete(taskAfterSubProcess.Id);
		rule.assertScenarioEnded();
	  }

	}

}