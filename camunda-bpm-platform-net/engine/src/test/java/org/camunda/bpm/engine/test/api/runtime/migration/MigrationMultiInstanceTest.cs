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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using MultiInstanceProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MultiInstanceProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationMultiInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationMultiInstanceTest()
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


	  public const string NUMBER_OF_INSTANCES = "nrOfInstances";
	  public const string NUMBER_OF_ACTIVE_INSTANCES = "nrOfActiveInstances";
	  public const string NUMBER_OF_COMPLETED_INSTANCES = "nrOfCompletedInstances";
	  public const string LOOP_COUNTER = "loopCounter";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMultiInstanceTask()
	  public virtual void testMigrateParallelMultiInstanceTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask"))).child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().up().done());

		ActivityInstance[] userTaskInstances = testHelper.snapshotBeforeMigration.ActivityTree.getActivityInstances("userTask");

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("userTask", testHelper.getSingleActivityInstanceBeforeMigration(miBodyOf("userTask")).Id).activity("userTask", userTaskInstances[0].Id).activity("userTask", userTaskInstances[1].Id).activity("userTask", userTaskInstances[2].Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(3, migratedTasks.Count);
		foreach (Task migratedTask in migratedTasks)
		{
		  assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);
		}

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMultiInstanceTasksVariables()
	  public virtual void testMigrateParallelMultiInstanceTasksVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		IList<Task> tasksBeforeMigration = rule.TaskService.createTaskQuery().list();
		IDictionary<string, int> loopCounterDistribution = new Dictionary<string, int>();
		foreach (Task task in tasksBeforeMigration)
		{
		  int? loopCounter = (int?) rule.TaskService.getVariable(task.Id, LOOP_COUNTER);
		  loopCounterDistribution[task.Id] = loopCounter.Value;
		}

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Task> tasks = testHelper.snapshotAfterMigration.Tasks;
		Task firstTask = tasks[0];
		Assert.assertEquals(3, rule.TaskService.getVariable(firstTask.Id, NUMBER_OF_INSTANCES));
		Assert.assertEquals(3, rule.TaskService.getVariable(firstTask.Id, NUMBER_OF_ACTIVE_INSTANCES));
		Assert.assertEquals(0, rule.TaskService.getVariable(firstTask.Id, NUMBER_OF_COMPLETED_INSTANCES));

		foreach (Task task in tasks)
		{
		  int? loopCounter = (int?) rule.TaskService.getVariable(task.Id, LOOP_COUNTER);
		  Assert.assertNotNull(loopCounter);
		  Assert.assertEquals(loopCounterDistribution[task.Id], loopCounter);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMultiInstancePartiallyComplete()
	  public virtual void testMigrateParallelMultiInstancePartiallyComplete()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeAnyTask("userTask");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask"))).child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().up().done());

		ActivityInstance[] userTaskInstances = testHelper.snapshotBeforeMigration.ActivityTree.getActivityInstances("userTask");

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("userTask", testHelper.getSingleActivityInstanceBeforeMigration(miBodyOf("userTask")).Id).activity("userTask", userTaskInstances[0].Id).activity("userTask", userTaskInstances[1].Id).transition("userTask").done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(2, migratedTasks.Count);
		foreach (Task migratedTask in migratedTasks)
		{
		  assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);
		}

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMiBodyRemoveSubprocess()
	  public virtual void testMigrateParallelMiBodyRemoveSubprocess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("subProcess"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(miBodyOf("subProcess"), "Cannot remove the inner activity of a multi-instance body when the body is mapped");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelMiBodyAddSubprocess()
	  public virtual void testMigrateParallelMiBodyAddSubprocess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("subProcess")).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(miBodyOf("userTask"), "Must map the inner activity of a multi-instance body when the body is mapped");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequentialMultiInstanceTask()
	  public virtual void testMigrateSequentialMultiInstanceTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask"))).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("userTask", testHelper.getSingleActivityInstanceBeforeMigration(miBodyOf("userTask")).Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.completeTask("userTask");
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequentialMultiInstanceTasksVariables()
	  public virtual void testMigrateSequentialMultiInstanceTasksVariables()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		Task task = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertEquals(3, rule.TaskService.getVariable(task.Id, NUMBER_OF_INSTANCES));
		Assert.assertEquals(1, rule.TaskService.getVariable(task.Id, NUMBER_OF_ACTIVE_INSTANCES));
		Assert.assertEquals(0, rule.TaskService.getVariable(task.Id, NUMBER_OF_COMPLETED_INSTANCES));
		Assert.assertEquals(0, rule.TaskService.getVariable(task.Id, NUMBER_OF_COMPLETED_INSTANCES));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequentialMultiInstancePartiallyComplete()
	  public virtual void testMigrateSequentialMultiInstancePartiallyComplete()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeAnyTask("userTask");
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration(miBodyOf("userTask"))).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginMiBody("userTask", testHelper.getSingleActivityInstanceBeforeMigration(miBodyOf("userTask")).Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		// and it is possible to successfully complete the migrated instance
		testHelper.completeTask("userTask");
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequenatialMiBodyRemoveSubprocess()
	  public virtual void testMigrateSequenatialMiBodyRemoveSubprocess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("subProcess"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(miBodyOf("subProcess"), "Cannot remove the inner activity of a multi-instance body when the body is mapped");
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateSequentialMiBodyAddSubprocess()
	  public virtual void testMigrateSequentialMiBodyAddSubprocess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_SUBPROCESS_PROCESS);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("subProcess")).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(miBodyOf("userTask"), "Must map the inner activity of a multi-instance body when the body is mapped");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateParallelToSequential()
	  public virtual void testMigrateParallelToSequential()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.SEQ_MI_ONE_TASK_PROCESS);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities(miBodyOf("userTask"), miBodyOf("userTask")).mapActivities("userTask", "userTask").build();
		  fail("Should not succeed");
		}
		catch (MigrationPlanValidationException e)
		{
		  assertThat(e.ValidationReport).hasInstructionFailures(miBodyOf("userTask"), "Activities have incompatible types (ParallelMultiInstanceActivityBehavior is not " + "compatible with SequentialMultiInstanceActivityBehavior)");
		}
	  }

	  protected internal virtual string miBodyOf(string activityId)
	  {
		return activityId + BpmnParse.MULTI_INSTANCE_BODY_ID_SUFFIX;
	  }

	}

}