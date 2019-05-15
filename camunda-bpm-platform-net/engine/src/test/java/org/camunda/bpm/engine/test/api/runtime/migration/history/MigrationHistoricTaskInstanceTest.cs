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
namespace org.camunda.bpm.engine.test.api.runtime.migration.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;

	using HistoricTaskInstance = org.camunda.bpm.engine.history.HistoricTaskInstance;
	using HistoricTaskInstanceQuery = org.camunda.bpm.engine.history.HistoricTaskInstanceQuery;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationHistoricTaskInstanceTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationHistoricTaskInstanceTest()
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

	  protected internal RuntimeService runtimeService;
	  protected internal HistoryService historyService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		historyService = rule.HistoryService;
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateHistoryUserTaskInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateHistoryUserTaskInstance()
	  {
		//given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId("Process", "Process2").changeElementId("userTask", "userTask2"));

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		HistoricTaskInstanceQuery sourceHistoryTaskInstanceQuery = historyService.createHistoricTaskInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		HistoricTaskInstanceQuery targetHistoryTaskInstanceQuery = historyService.createHistoricTaskInstanceQuery().processDefinitionId(targetProcessDefinition.Id);

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstance.Id);

		//when
		assertEquals(1, sourceHistoryTaskInstanceQuery.count());
		assertEquals(0, targetHistoryTaskInstanceQuery.count());
		ProcessInstanceQuery sourceProcessInstanceQuery = runtimeService.createProcessInstanceQuery().processDefinitionId(sourceProcessDefinition.Id);
		runtimeService.newMigration(migrationPlan).processInstanceQuery(sourceProcessInstanceQuery).execute();

		//then
		assertEquals(0, sourceHistoryTaskInstanceQuery.count());
		assertEquals(1, targetHistoryTaskInstanceQuery.count());

		HistoricTaskInstance instance = targetHistoryTaskInstanceQuery.singleResult();
		assertEquals(targetProcessDefinition.Key, instance.ProcessDefinitionKey);
		assertEquals(targetProcessDefinition.Id, instance.ProcessDefinitionId);
		assertEquals("userTask2", instance.TaskDefinitionKey);
		assertEquals(activityInstance.getActivityInstances("userTask")[0].Id, instance.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void testMigrateWithSubTask()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void testMigrateWithSubTask()
	  {
		//given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = runtimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		Task subTask = taskService.newTask();
		subTask.ParentTaskId = task.Id;
		taskService.saveTask(subTask);

		// when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then the historic sub task instance is still the same
		HistoricTaskInstance historicSubTaskAfterMigration = historyService.createHistoricTaskInstanceQuery().taskId(subTask.Id).singleResult();

		Assert.assertNotNull(historicSubTaskAfterMigration);
		Assert.assertNull(historicSubTaskAfterMigration.ProcessDefinitionId);
		Assert.assertNull(historicSubTaskAfterMigration.ProcessDefinitionKey);
		Assert.assertNull(historicSubTaskAfterMigration.ExecutionId);
		Assert.assertNull(historicSubTaskAfterMigration.ActivityInstanceId);
	  }
	}

}