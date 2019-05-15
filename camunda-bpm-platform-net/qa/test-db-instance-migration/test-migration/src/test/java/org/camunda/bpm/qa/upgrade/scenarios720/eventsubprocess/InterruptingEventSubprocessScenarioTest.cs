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
namespace org.camunda.bpm.qa.upgrade.scenarios720.eventsubprocess
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

	[ScenarioUnderTest("InterruptingEventSubprocessScenario"), Origin("7.2.0")]
	public class InterruptingEventSubprocessScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.1") public void testCompletion()
	  [ScenarioUnderTest("init.1")]
	  public virtual void testCompletion()
	  {
		// given
		Task task = rule.taskQuery().singleResult();

		// when
		rule.TaskService.complete(task.Id);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.2") public void testActivityInstanceTree()
	  [ScenarioUnderTest("init.2")]
	  public virtual void testActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("eventSubProcessTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("init.3") public void testDeletion()
	  [ScenarioUnderTest("init.3")]
	  public virtual void testDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

	}

}