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
namespace org.camunda.bpm.qa.upgrade.scenarios730.compensation
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
	/// </summary>
	[ScenarioUnderTest("NestedMultiInstanceCompensationScenario"), Origin("7.3.0")]
	public class NestedMultiInstanceCompensationScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwInner.1") public void testInitThrowInnerCompletion()
	  [ScenarioUnderTest("init.throwInner.1")]
	  public virtual void testInitThrowInnerCompletion()
	  {
		// given
		IList<Task> undoTasks = rule.taskQuery().list();

		// when
		foreach (Task undoTask in undoTasks)
		{
		  rule.TaskService.complete(undoTask.Id);
		}

		// then the process has successfully completed
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwInner.2") public void testInitThrowInnerDeletion()
	  [ScenarioUnderTest("init.throwInner.2")]
	  public virtual void testInitThrowInnerDeletion()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwInner.3") public void testInitThrowInnerActivityInstanceTree()
	  [ScenarioUnderTest("init.throwInner.3")]
	  public virtual void testInitThrowInnerActivityInstanceTree()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		ActivityInstance tree = rule.RuntimeService.getActivityInstance(processInstance.Id);

		// then
		Assert.assertNotNull(tree);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginMiBody("subProcess").beginScope("subProcess").beginScope("throwCompensate").activity("undoTask").activity("undoTask").endScope().endScope().beginScope("subProcess").beginScope("throwCompensate").activity("undoTask").activity("undoTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwOuter.1") public void testInitThrowOuterCompletion()
	  [ScenarioUnderTest("init.throwOuter.1")]
	  public virtual void testInitThrowOuterCompletion()
	  {
		// given
		IList<Task> undoTasks = rule.taskQuery().list();

		// when
		foreach (Task undoTask in undoTasks)
		{
		  rule.TaskService.complete(undoTask.Id);
		}

		// then the process has successfully completed
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwOuter.2") public void testInitThrowOuterDeletion()
	  [ScenarioUnderTest("init.throwOuter.2")]
	  public virtual void testInitThrowOuterDeletion()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(processInstance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.throwOuter.3") public void testInitThrowOuterActivityInstanceTree()
	  [ScenarioUnderTest("init.throwOuter.3")]
	  public virtual void testInitThrowOuterActivityInstanceTree()
	  {
		// given
		ProcessInstance processInstance = rule.processInstance();

		// when
		ActivityInstance tree = rule.RuntimeService.getActivityInstance(processInstance.Id);

		// then
		Assert.assertNotNull(tree);
		assertThat(tree).hasStructure(describeActivityInstanceTree(processInstance.ProcessDefinitionId).beginScope("throwCompensate", "outerSubProcess").beginScope("innerSubProcess").beginScope("undoSubProcess").activity("undoTask").activity("undoTask").endScope().endScope().beginScope("innerSubProcess").beginScope("undoSubProcess").activity("undoTask").activity("undoTask").done());
	  }
	}

}