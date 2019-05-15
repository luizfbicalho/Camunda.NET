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
namespace org.camunda.bpm.qa.upgrade.scenarios720.multiinstance
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.qa.upgrade.util.ActivityInstanceAssert.describeActivityInstanceTree;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	[ScenarioUnderTest("MultiInstanceReceiveTaskScenario"), Origin("7.2.0")]
	public class MultiInstanceReceiveTaskScenarioTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.qa.upgrade.UpgradeTestRule rule = new org.camunda.bpm.qa.upgrade.UpgradeTestRule();
		public UpgradeTestRule rule = new UpgradeTestRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initParallel.1") public void testInitParallelCompletion()
	  [ScenarioUnderTest("initParallel.1")]
	  public virtual void testInitParallelCompletion()
	  {
		// when the receive task messages are correlated
		rule.messageCorrelation("Message").correlateAll();

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initParallel.2") public void testInitParallelActivityInstanceTree()
	  [ScenarioUnderTest("initParallel.2")]
	  public virtual void testInitParallelActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("miReceiveTask").activity("miReceiveTask").activity("miReceiveTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initParallel.3") public void testInitParallelDeletion()
	  [ScenarioUnderTest("initParallel.3")]
	  public virtual void testInitParallelDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-6408") @ScenarioUnderTest("initParallel.4") public void testInitParallelMigration()
	  [ScenarioUnderTest("initParallel.4")]
	  public virtual void testInitParallelMigration()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(instance.ProcessDefinitionId, instance.ProcessDefinitionId).mapEqualActivities().build();

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(instance.Id).execute();

		// then the receive task messages can be correlated
		rule.messageCorrelation("Message").correlateAll();
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initSequential.1") public void testInitSequentialCompletion()
	  [ScenarioUnderTest("initSequential.1")]
	  public virtual void testInitSequentialCompletion()
	  {
		// when the receive task messages are correlated
		for (int i = 0; i < 3; i++)
		{
		  rule.messageCorrelation("Message").correlate();
		}

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initSequential.2") public void testInitSequentialActivityInstanceTree()
	  [ScenarioUnderTest("initSequential.2")]
	  public virtual void testInitSequentialActivityInstanceTree()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(instance.Id);

		// then
		Assert.assertNotNull(activityInstance);
		assertThat(activityInstance).hasStructure(describeActivityInstanceTree(instance.ProcessDefinitionId).activity("miReceiveTask").done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @ScenarioUnderTest("initSequential.3") public void testInitSequentialDeletion()
	  [ScenarioUnderTest("initSequential.3")]
	  public virtual void testInitSequentialDeletion()
	  {
		// given
		ProcessInstance instance = rule.processInstance();

		// when
		rule.RuntimeService.deleteProcessInstance(instance.Id, null);

		// then
		rule.assertScenarioEnded();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore("CAM-6408") @ScenarioUnderTest("initSequential.4") public void testInitSequentialMigration()
	  [ScenarioUnderTest("initSequential.4")]
	  public virtual void testInitSequentialMigration()
	  {
		// given
		ProcessInstance instance = rule.processInstance();
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(instance.ProcessDefinitionId, instance.ProcessDefinitionId).mapEqualActivities().build();

		// when
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(instance.Id).execute();

		// then the receive task messages can be correlated
		for (int i = 0; i < 3; i++)
		{
		  rule.messageCorrelation("Message").correlate();
		}

		rule.assertScenarioEnded();
	  }

	}

}