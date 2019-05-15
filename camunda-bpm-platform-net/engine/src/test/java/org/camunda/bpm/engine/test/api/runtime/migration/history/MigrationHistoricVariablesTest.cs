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
namespace org.camunda.bpm.engine.test.api.runtime.migration.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;


	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using CompensationModels = org.camunda.bpm.engine.test.api.runtime.migration.models.CompensationModels;
	using MultiInstanceProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MultiInstanceProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationHistoricVariablesTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationHistoricVariablesTest()
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

	  protected internal static readonly BpmnModelInstance ONE_BOUNDARY_TASK = ModifiableBpmnModelInstance.modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent().message("Message").done();

	  protected internal static readonly BpmnModelInstance CONCURRENT_BOUNDARY_TASKS = ModifiableBpmnModelInstance.modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent().message("Message").moveToActivity("userTask2").boundaryEvent().message("Message").done();

	  protected internal static readonly BpmnModelInstance SUBPROCESS_CONCURRENT_BOUNDARY_TASKS = ModifiableBpmnModelInstance.modify(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS).activityBuilder("userTask1").boundaryEvent().message("Message").moveToActivity("userTask2").boundaryEvent().message("Message").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;
	  protected internal HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
		historyService = rule.HistoryService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void noHistoryUpdateOnSameStructureMigration()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void noHistoryUpdateOnSameStructureMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_BOUNDARY_TASK);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ONE_BOUNDARY_TASK);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree scopeExecution = executionTreeBeforeMigration.Executions[0];

		runtimeService.setVariableLocal(scopeExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then there is still one historic variable instance
		Assert.assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());

		// and no additional historic details
		Assert.assertEquals(1, historyService.createHistoricDetailQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL) public void noHistoryUpdateOnAddScopeMigration()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_FULL)]
	  public virtual void noHistoryUpdateOnAddScopeMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0].Parent;

		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then there is still one historic variable instance
		Assert.assertEquals(1, historyService.createHistoricVariableInstanceQuery().count());

		// and no additional historic details
		Assert.assertEquals(1, historyService.createHistoricDetailQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testMigrateHistoryVariableInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testMigrateHistoryVariableInstance()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		runtimeService.setVariable(processInstance.Id, "test", 3537);
		HistoricVariableInstance instance = historyService.createHistoricVariableInstanceQuery().singleResult();

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask", "userTask").build();

		//when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		//then
		HistoricVariableInstance migratedInstance = historyService.createHistoricVariableInstanceQuery().singleResult();
		assertEquals(targetDefinition.Key, migratedInstance.ProcessDefinitionKey);
		assertEquals(targetDefinition.Id, migratedInstance.ProcessDefinitionId);
		assertEquals(instance.ActivityInstanceId, migratedInstance.ActivityInstanceId);
		assertEquals(instance.ExecutionId, migratedInstance.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testMigrateHistoryVariableInstanceMultiInstance()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testMigrateHistoryVariableInstanceMultiInstance()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(MultiInstanceProcessModels.PAR_MI_SUBPROCESS_PROCESS);

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapEqualActivities().build();

		//when
		runtimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		//then
		IList<HistoricVariableInstance> migratedVariables = historyService.createHistoricVariableInstanceQuery().list();
		Assert.assertEquals(6, migratedVariables.Count); // 3 loop counter + nrOfInstance + nrOfActiveInstances + nrOfCompletedInstances

		foreach (HistoricVariableInstance variable in migratedVariables)
		{
		  assertEquals(targetDefinition.Key, variable.ProcessDefinitionKey);
		  assertEquals(targetDefinition.Id, variable.ProcessDefinitionId);

		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testMigrateEventScopeVariable()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testMigrateEventScopeVariable()
	  {
		//given
		ProcessDefinition sourceDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);
		ProcessDefinition targetDefinition = testHelper.deployAndGetDefinition(CompensationModels.COMPENSATION_ONE_TASK_SUBPROCESS_MODEL);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceDefinition.Id, targetDefinition.Id).mapActivities("userTask2", "userTask2").mapActivities("subProcess", "subProcess").mapActivities("compensationBoundary", "compensationBoundary").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceDefinition.Id);

		Execution subProcessExecution = runtimeService.createExecutionQuery().activityId("userTask1").singleResult();

		runtimeService.setVariableLocal(subProcessExecution.Id, "foo", "bar");

		testHelper.completeTask("userTask1");

		Execution eventScopeExecution = runtimeService.createExecutionQuery().activityId("subProcess").singleResult();
		HistoricVariableInstance eventScopeVariable = historyService.createHistoricVariableInstanceQuery().executionIdIn(eventScopeExecution.Id).singleResult();

		//when
		runtimeService.newMigration(migrationPlan).processInstanceIds(processInstance.Id).execute();

		// then
		HistoricVariableInstance historicVariableInstance = historyService.createHistoricVariableInstanceQuery().variableId(eventScopeVariable.Id).singleResult();
		Assert.assertEquals(targetDefinition.Id, historicVariableInstance.ProcessDefinitionId);
	  }
	}

}