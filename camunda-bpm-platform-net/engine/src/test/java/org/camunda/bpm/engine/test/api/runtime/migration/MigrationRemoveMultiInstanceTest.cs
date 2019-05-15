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
namespace org.camunda.bpm.engine.test.api.runtime.migration
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using MultiInstanceProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MultiInstanceProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationRemoveMultiInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationRemoveMultiInstanceTest()
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
//ORIGINAL LINE: @Test public void testRemoveParallelMultiInstanceBody()
	  public virtual void testRemoveParallelMultiInstanceBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().done());

		ActivityInstance[] userTaskInstances = testHelper.snapshotBeforeMigration.ActivityTree.getActivityInstances("userTask");

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", userTaskInstances[0].Id).activity("userTask", userTaskInstances[1].Id).activity("userTask", userTaskInstances[2].Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(3, migratedTasks.Count);

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }



//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveParallelMultiInstanceBodyVariables()
	  public virtual void testRemoveParallelMultiInstanceBodyVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().variableName("nrOfInstances").count());

		// the MI body variables are gone
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().variableName("nrOfInstances").count());
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().variableName("nrOfActiveInstances").count());
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().variableName("nrOfCompletedInstances").count());

		// and the loop counters are still there (because they logically belong to the inner activity instances)
		Assert.assertEquals(3, rule.RuntimeService.createVariableInstanceQuery().variableName("loopCounter").count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveParallelMultiInstanceBodyScope()
	  public virtual void testRemoveParallelMultiInstanceBodyScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		ActivityInstance[] subProcessInstances = testHelper.snapshotBeforeMigration.ActivityTree.getActivityInstances("subProcess");

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).concurrent().noScope().child("userTask").scope().id(testHelper.getSingleExecutionIdForActivity(subProcessInstances[0], "subProcess")).up().up().child(null).concurrent().noScope().child("userTask").scope().id(testHelper.getSingleExecutionIdForActivity(subProcessInstances[1], "subProcess")).up().up().child(null).concurrent().noScope().child("userTask").scope().id(testHelper.getSingleExecutionIdForActivity(subProcessInstances[2], "subProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess", subProcessInstances[0].Id).activity("userTask", subProcessInstances[0].getActivityInstances("userTask")[0].Id).endScope().beginScope("subProcess", subProcessInstances[1].Id).activity("userTask", subProcessInstances[1].getActivityInstances("userTask")[0].Id).endScope().beginScope("subProcess", subProcessInstances[2].Id).activity("userTask", subProcessInstances[2].getActivityInstances("userTask")[0].Id).endScope().done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(3, migratedTasks.Count);

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveParallelMultiInstanceBodyOneInstanceFinished()
	  public virtual void testRemoveParallelMultiInstanceBodyOneInstanceFinished()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Task firstTask = rule.TaskService.createTaskQuery().listPage(0, 1).get(0);
		rule.TaskService.complete(firstTask.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().done());

		ActivityInstance[] userTaskInstances = testHelper.snapshotBeforeMigration.ActivityTree.getActivityInstances("userTask");

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", userTaskInstances[0].Id).activity("userTask", userTaskInstances[1].Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(2, migratedTasks.Count);

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSequentialMultiInstanceBody()
	  public virtual void testRemoveSequentialMultiInstanceBody()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveSequentialMultiInstanceBodyVariables()
	  public virtual void testRemoveSequentialMultiInstanceBodyVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then all MI variables are gone
		Assert.assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemovSequentialMultiInstanceBodyScope()
	  public virtual void testRemovSequentialMultiInstanceBodyScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		ActivityInstance subProcessInstance = testHelper.getSingleActivityInstanceBeforeMigration("subProcess");

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("subProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess", subProcessInstance.Id).activity("userTask", subProcessInstance.getActivityInstances("userTask")[0].Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }
	}

}