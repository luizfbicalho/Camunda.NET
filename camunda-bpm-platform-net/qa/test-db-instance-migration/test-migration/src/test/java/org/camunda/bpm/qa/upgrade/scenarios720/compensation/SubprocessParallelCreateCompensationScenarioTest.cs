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
namespace org.camunda.bpm.qa.upgrade.scenarios720.compensation
{
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("SubprocessParallelCreateCompensationScenario"), Origin("7.2.0")]
	public class SubprocessParallelCreateCompensationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testInitCompletionCase1()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testInitCompletionCase1()
	  {
		// given
		Task afterUserTask1 = rule.taskQuery().taskDefinitionKey("afterUserTask1").singleResult();
		Task userTask2 = rule.taskQuery().taskDefinitionKey("userTask2").singleResult();

		// when the subprocess is completed by first compacting the concurrent execution
		// in which context a compensation subscription was already created
		rule.TaskService.complete(afterUserTask1.Id);

		// and then userTask2 is completed
		rule.TaskService.complete(userTask2.Id);

		// and compensation is thrown
		Task beforeCompensationTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensationTask.Id);

		// then there is are two active compensation handler task
		Assert.assertEquals(2, rule.taskQuery().count());
		Task undoTask1 = rule.taskQuery().taskDefinitionKey("undoTask1").singleResult();
		Task undoTask2 = rule.taskQuery().taskDefinitionKey("undoTask2").singleResult();
		Assert.assertNotNull(undoTask1);
		Assert.assertNotNull(undoTask2);

		// and they can be completed such that the process instance ends successfully
		rule.TaskService.complete(undoTask1.Id);
		rule.TaskService.complete(undoTask2.Id);

		Task afterCompensateTask = rule.taskQuery().taskDefinitionKey("afterCompensate").singleResult();
		Assert.assertNotNull(afterCompensateTask);

		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.2") public void testInitCompletionCase2()
	  [ScenarioUnderTest("init.2")]
	  public virtual void testInitCompletionCase2()
	  {
		// given
		Task afterUserTask1 = rule.taskQuery().taskDefinitionKey("afterUserTask1").singleResult();
		Task userTask2 = rule.taskQuery().taskDefinitionKey("userTask2").singleResult();

		// when the task is completed first that belongs to an execution in which context
		// no event subscription was created yet
		rule.TaskService.complete(userTask2.Id);

		// and then afterUserTask1 is completed
		rule.TaskService.complete(afterUserTask1.Id);

		// and compensation is thrown
		Task beforeCompensationTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensationTask.Id);

		// then there is are two active compensation handler task
		Assert.assertEquals(2, rule.taskQuery().count());
		Task undoTask1 = rule.taskQuery().taskDefinitionKey("undoTask1").singleResult();
		Task undoTask2 = rule.taskQuery().taskDefinitionKey("undoTask2").singleResult();
		Assert.assertNotNull(undoTask2);
		Assert.assertNotNull(undoTask1);

		// and they can be completed such that the process instance ends successfully
		rule.TaskService.complete(undoTask1.Id);
		rule.TaskService.complete(undoTask2.Id);

		Task afterCompensateTask = rule.taskQuery().taskDefinitionKey("afterCompensate").singleResult();
		Assert.assertNotNull(afterCompensateTask);

		rule.TaskService.complete(afterCompensateTask.Id);
		rule.assertScenarioEnded();
	  }
	}


}