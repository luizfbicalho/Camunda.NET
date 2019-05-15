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
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.describeActivityInstanceTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.describeExecutionTree;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.MigratingProcessInstanceValidationReportAssert.assertThat;

	using DelegateTask = org.camunda.bpm.engine.@delegate.DelegateTask;
	using TaskListener = org.camunda.bpm.engine.@delegate.TaskListener;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using CamundaTaskListener = org.camunda.bpm.model.bpmn.instance.camunda.CamundaTaskListener;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationUserTaskTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationUserTaskTest()
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
//ORIGINAL LINE: @Test public void testUserTaskMigrationInProcessDefinitionScope()
	  public virtual void testUserTaskMigrationInProcessDefinitionScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then

		// the entities were migrated
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		Task task = testHelper.snapshotBeforeMigration.getTaskForKey("userTask");
		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertEquals(task.Id, migratedTask.Id);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUserTaskMigrationInSubProcessScope()
	  public virtual void testUserTaskMigrationInSubProcessScope()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then

		// the entities were migrated
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask")).done());

		Task task = testHelper.snapshotBeforeMigration.getTaskForKey("userTask");
		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertEquals(task.Id, migratedTask.Id);
		Assert.assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConcurrentUserTaskMigration()
	  public virtual void testConcurrentUserTaskMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then

		// the entities were migrated
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask1").concurrent().noScope().up().child("userTask2").concurrent().noScope().done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		Assert.assertEquals(2, migratedTasks.Count);

		foreach (Task migratedTask in migratedTasks)
		{
		  Assert.assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);
		}

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateWhenNotAllActivityInstancesAreMapped()
	  public virtual void testCannotMigrateWhenNotAllActivityInstancesAreMapped()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").build();


		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should not succeed because the userTask2 instance is not mapped");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  assertThat(e.ValidationReport).hasActivityInstanceFailures("userTask2", "There is no migration instruction for this instance's activity");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateWhenNotAllTransitionInstancesAreMapped()
	  public virtual void testCannotMigrateWhenNotAllTransitionInstancesAreMapped()
	  {
		// given
		BpmnModelInstance model = ModifiableBpmnModelInstance.modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").camundaAsyncBefore().moveToActivity("userTask2").camundaAsyncBefore().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").build();


		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should not succeed because the userTask2 instance is not mapped");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask2", "There is no migration instruction for this instance's activity");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeActivityId()
	  public virtual void testChangeActivityId()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask2").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask1").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask2").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask2");
		Assert.assertNotNull(migratedTask);
		Assert.assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);
		Assert.assertEquals("userTask2", migratedTask.TaskDefinitionKey);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateWithSubTask()
	  public virtual void testMigrateWithSubTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Task task = rule.TaskService.createTaskQuery().singleResult();
		Task subTask = rule.TaskService.newTask();
		subTask.ParentTaskId = task.Id;
		rule.TaskService.saveTask(subTask);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the sub task properties have not been updated (i.e. subtask should not reference the process instance/definition now)
		Task subTaskAfterMigration = rule.TaskService.createTaskQuery().taskId(subTask.Id).singleResult();
		Assert.assertNull(subTaskAfterMigration.ProcessDefinitionId);
		Assert.assertNull(subTaskAfterMigration.ProcessInstanceId);
		Assert.assertNull(subTaskAfterMigration.TaskDefinitionKey);

		// the tasks can be completed and the process can be ended
		rule.TaskService.complete(subTask.Id);
		rule.TaskService.complete(task.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);

		if (!rule.ProcessEngineConfiguration.HistoryLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_NONE))
		{
		  rule.HistoryService.deleteHistoricTaskInstance(subTaskAfterMigration.Id);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAccessModelInTaskListenerAfterMigration()
	  public virtual void testAccessModelInTaskListenerAfterMigration()
	  {
		BpmnModelInstance targetModel = modify(ProcessModels.ONE_TASK_PROCESS).changeElementId("userTask", "newUserTask");
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		addTaskListener(targetModel, "newUserTask", org.camunda.bpm.engine.@delegate.TaskListener_Fields.EVENTNAME_ASSIGNMENT, typeof(AccessModelInstanceTaskListener).FullName);

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetModel);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "newUserTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// when
		Task task = rule.TaskService.createTaskQuery().singleResult();

		rule.TaskService.setAssignee(task.Id, "foo");

		// then the task listener was able to access the bpmn model instance and set a variable
		string variableValue = (string) rule.RuntimeService.getVariable(processInstance.Id, AccessModelInstanceTaskListener.VARIABLE_NAME);
		Assert.assertEquals("newUserTask", variableValue);

	  }

	  protected internal static void addTaskListener(BpmnModelInstance targetModel, string activityId, string @event, string className)
	  {
		CamundaTaskListener taskListener = targetModel.newInstance(typeof(CamundaTaskListener));
		taskListener.CamundaClass = className;
		taskListener.CamundaEvent = @event;

		UserTask task = targetModel.getModelElementById(activityId);
		task.builder().addExtensionElement(taskListener);
	  }

	  public class AccessModelInstanceTaskListener : TaskListener
	  {

		public const string VARIABLE_NAME = "userTaskId";

		public virtual void notify(DelegateTask delegateTask)
		{
		  UserTask userTask = delegateTask.BpmnModelElementInstance;
		  delegateTask.setVariable(VARIABLE_NAME, userTask.Id);
		}

	  }

	}

}