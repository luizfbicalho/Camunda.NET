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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using TimerCatchModels = org.camunda.bpm.engine.test.api.runtime.migration.models.TimerCatchModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ClockTestUtil = org.camunda.bpm.engine.test.util.ClockTestUtil;
	using DateTime = org.joda.time.DateTime;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationTimerCatchEventTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationTimerCatchEventTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testHelper = new MigrationTestRule(rule);
			ruleChain = RuleChain.outerRule(rule).around(testHelper);
		}


	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJob()
	  public virtual void testMigrateJob()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "timerCatch");

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("timerCatch").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("timerCatch")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("timerCatch", testHelper.getSingleActivityInstanceBeforeMigration("timerCatch").Id).done());

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobChangeActivityId()
	  public virtual void testMigrateJobChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(TimerCatchModels.ONE_TIMER_CATCH_PROCESS).changeElementId("timerCatch", "newTimerCatch"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "newTimerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "newTimerCatch");

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobPreserveTimerConfiguration()
	  public virtual void testMigrateJobPreserveTimerConfiguration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("timerCatch").timerWithDuration("PT50M").userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "timerCatch");

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobUpdateTimerConfiguration()
	  public virtual void testMigrateJobUpdateTimerConfiguration()
	  {
		// given
		ClockTestUtil.setClockToDateWithoutMilliseconds();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().intermediateCatchEvent("timerCatch").timerWithDuration("PT50M").userTask("userTask").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "timerCatch").updateEventTrigger().build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		DateTime newDueDate = (new DateTime(ClockUtil.CurrentTime)).plusMinutes(50).toDate();
		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "timerCatch", newDueDate);

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobChangeProcessKey()
	  public virtual void testMigrateJobChangeProcessKey()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(TimerCatchModels.ONE_TIMER_CATCH_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "timerCatch");

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateJobAddParentScope()
	  public virtual void testMigrateJobAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.ONE_TIMER_CATCH_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(TimerCatchModels.SUBPROCESS_TIMER_CATCH_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("timerCatch", "timerCatch").build();

		// when
		ProcessInstance processInstance = testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated(testHelper.snapshotBeforeMigration.Jobs[0], "timerCatch");

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("timerCatch").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("timerCatch")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("timerCatch", testHelper.getSingleActivityInstanceBeforeMigration("timerCatch").Id).done());

		// and it is possible to trigger the event
		Job jobAfterMigration = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(jobAfterMigration.Id);

		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(processInstance.Id);
	  }
	}

}