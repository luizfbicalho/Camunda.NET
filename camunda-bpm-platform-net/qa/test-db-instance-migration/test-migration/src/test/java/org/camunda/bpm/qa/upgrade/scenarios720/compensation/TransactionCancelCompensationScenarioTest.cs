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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	[ScenarioUnderTest("TransactionCancelCompensationScenario"), Origin("7.2.0")]
	public class TransactionCancelCompensationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testInitCompletion()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testInitCompletion()
	  {
		// when compensation is thrown
		Task beforeCompensationTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensationTask.Id);

		// then there is an active compensation handler task
		Task compensationHandlerTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(compensationHandlerTask);
		Assert.assertEquals("undoTask", compensationHandlerTask.TaskDefinitionKey);

		// and it can be completed such that the process instance ends successfully
		rule.TaskService.complete(compensationHandlerTask.Id);

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		Assert.assertEquals("afterCancel", afterCompensateTask.TaskDefinitionKey);

		rule.TaskService.complete(afterCompensateTask.Id);

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.2") public void testInitDeletion()
	  [ScenarioUnderTest("init.2")]
	  public virtual void testInitDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when compensation is thrown
		Task beforeCompensationTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensationTask.Id);

		// then the process instance can be deleted
		rule.RuntimeService.deleteProcessInstance(instance.Id, "");

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.3") public void testInitActivityInstanceTree()
	  [ScenarioUnderTest("init.3")]
	  public virtual void testInitActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when compensation is thrown
		Task beforeCompensationTask = rule.taskQuery().singleResult();
		rule.TaskService.complete(beforeCompensationTask.Id);

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).beginScope("tx").activity("txEnd").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.triggerCompensation.1") public void testInitTriggerCompensationCompletion()
	  [ScenarioUnderTest("init.triggerCompensation.1")]
	  public virtual void testInitTriggerCompensationCompletion()
	  {
		// given active compensation
		Task compensationHandlerTask = rule.taskQuery().singleResult();

		// then it is possible to complete compensation and the follow-up task
		rule.TaskService.complete(compensationHandlerTask.Id);

		Task afterCompensateTask = rule.taskQuery().singleResult();
		Assert.assertNotNull(afterCompensateTask);
		Assert.assertEquals("afterCancel", afterCompensateTask.TaskDefinitionKey);

		rule.TaskService.complete(afterCompensateTask.Id);

		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.triggerCompensation.2") public void testInitTriggerCompensationDeletion()
	  [ScenarioUnderTest("init.triggerCompensation.2")]
	  public virtual void testInitTriggerCompensationDeletion()
	  {
		// given active compensation
		ProcessInstance instance = rule.processInstance();

		// then the process instance can be deleted
		rule.RuntimeService.deleteProcessInstance(instance.Id, "");

		// and the process is ended
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.triggerCompensation.3") public void testInitTriggerCompensationActivityInstanceTree()
	  [ScenarioUnderTest("init.triggerCompensation.3")]
	  public virtual void testInitTriggerCompensationActivityInstanceTree()
	  {
		// given active compensation
		ProcessInstance instance = rule.processInstance();

		// then the activity instance tree is meaningful
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("catchCancelTx").beginScope("tx").activity("undoTask").done());
	  }
	}

}