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
namespace org.camunda.bpm.qa.upgrade.scenarios730.compensation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using HistoricActivityInstance = org.camunda.bpm.engine.history.HistoricActivityInstance;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// Tests that the 7.2-to-current migration logic (where an event subprocess was no scope)
	/// does not precede over 7.3-to-current migration logic (where a throwing compensation event was no scope)
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	[ScenarioUnderTest("InterruptingEventSubProcessCompensationScenario"), Origin("7.3.0")]
	public class InterruptingEventSubProcessCompensationSenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwCompensate.1") public void testInitThrowCompensateCompletion()
	  [ScenarioUnderTest("init.throwCompensate.1")]
	  public virtual void testInitThrowCompensateCompletion()
	  {
		// given
		Task undoTask = rule.taskQuery().singleResult();

		// when
		rule.TaskService.complete(undoTask.Id);

		// then it is possible to complete the process successfully
		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		Assert.assertEquals("afterCompensate", afterCompensateTask.TaskDefinitionKey);

		rule.TaskService.complete(afterCompensateTask.Id);

		rule.assertScenarioEnded();
	  }

	  /// <summary>
	  /// Does not work because in PvmAtomicOperationActivityInstanceEnd we don't correctly
	  /// detect that the compensation throw event has no dedicated scope execution, so we restore
	  /// activity instance IDs wrongly before firing the history listener.
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test @ScenarioUnderTest("init.throwCompensate.2") public void testInitThrowCompensateCompletionHistory()
	  [ScenarioUnderTest("init.throwCompensate.2")]
	  public virtual void testInitThrowCompensateCompletionHistory()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();
		Task undoTask = rule.taskQuery().singleResult();

		// when
		rule.TaskService.complete(undoTask.Id);
		Task afterCompensateTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(afterCompensateTask.Id);

		// then the historic throw compensate instance has an end time
		HistoricActivityInstance throwCompensateInstance = rule.HistoryService.createHistoricActivityInstanceQuery().processInstanceId(processInstance.Id).activityId("throwCompensate").singleResult();

		Assert.assertNotNull(throwCompensateInstance.EndTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwCompensate.3") public void testInitThrowCompensateDeletion()
	  [ScenarioUnderTest("init.throwCompensate.3")]
	  public virtual void testInitThrowCompensateDeletion()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwCompensate.4") public void testInitThrowCompensateActivityInstanceTree()
	  [ScenarioUnderTest("init.throwCompensate.4")]
	  public virtual void testInitThrowCompensateActivityInstanceTree()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		ActivityInstance tree = rule.RuntimeService.getActivityInstance(processInstance.Id);

		// then
		Assert.assertNotNull(tree);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("throwCompensate").activity("undoTask").done());
	  }


	}

}