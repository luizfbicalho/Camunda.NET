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


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using AsyncProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.AsyncProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ExecutionTree = org.camunda.bpm.engine.test.util.ExecutionTree;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using SerializationDataFormats = org.camunda.bpm.engine.variable.Variables.SerializationDataFormats;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationVariablesTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationVariablesTest()
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
			ruleChain = RuleChain.outerRule(bootstrapRule).around(rule).around(testHelper);
		}


	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			configuration.JavaSerializationFormatEnabled = true;
			return configuration;
		  }
	  }
	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule(bootstrapRule);
	  protected internal MigrationTestRule testHelper;

	  protected internal static readonly BpmnModelInstance ONE_BOUNDARY_TASK = ModifiableBpmnModelInstance.modify(ProcessModels.ONE_TASK_PROCESS).activityBuilder("userTask").boundaryEvent().message("Message").done();

	  protected internal static readonly BpmnModelInstance CONCURRENT_BOUNDARY_TASKS = ModifiableBpmnModelInstance.modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent().message("Message").moveToActivity("userTask2").boundaryEvent().message("Message").done();

	  protected internal static readonly BpmnModelInstance SUBPROCESS_CONCURRENT_BOUNDARY_TASKS = ModifiableBpmnModelInstance.modify(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS).activityBuilder("userTask1").boundaryEvent().message("Message").moveToActivity("userTask2").boundaryEvent().message("Message").done();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(rule).around(testHelper);
	  public RuleChain ruleChain;

	  protected internal RuntimeService runtimeService;
	  protected internal TaskService taskService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initServices()
	  public virtual void initServices()
	  {
		runtimeService = rule.RuntimeService;
		taskService = rule.TaskService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeExecutionInScopeActivity()
	  public virtual void testVariableAtScopeExecutionInScopeActivity()
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

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInScopeActivity()
	  public virtual void testVariableAtConcurrentExecutionInScopeActivity()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree concurrentExecution = executionTreeBeforeMigration.Executions[0];

		runtimeService.setVariableLocal(concurrentExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeExecutionInNonScopeActivity()
	  public virtual void testVariableAtScopeExecutionInNonScopeActivity()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id, Variables.createVariables().putValue("foo", 42));

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInNonScopeActivity()
	  public virtual void testVariableAtConcurrentExecutionInNonScopeActivity()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree concurrentExecution = executionTreeBeforeMigration.Executions[0];

		runtimeService.setVariableLocal(concurrentExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInScopeActivityAddParentScope()
	  public virtual void testVariableAtConcurrentExecutionInScopeActivityAddParentScope()
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

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		ExecutionTree userTask1CCExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0].Parent;

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		ActivityInstance subProcessInstance = testHelper.getSingleActivityInstanceAfterMigration("subProcess");
		// for variables at concurrent executions that are parent of a leaf-scope-execution, the activity instance is
		// the activity instance id of the parent activity instance (which is probably a bug)
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id, subProcessInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInScopeActivityRemoveParentScope()
	  public virtual void testVariableAtConcurrentExecutionInScopeActivityRemoveParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0].Parent;

		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		ExecutionTree userTask1CCExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0].Parent;

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		// for variables at concurrent executions that are parent of a leaf-scope-execution, the activity instance is
		// the activity instance id of the parent activity instance (which is probably a bug)
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id, processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInNonScopeActivityAddParentScope()
	  public virtual void testVariableAtConcurrentExecutionInNonScopeActivityAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];

		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		ExecutionTree userTask1CCExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0];

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInNonScopeActivityRemoveParentScope()
	  public virtual void testVariableAtConcurrentExecutionInNonScopeActivityRemoveParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];

		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		ExecutionTree userTask1CCExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0];

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1CCExecutionAfter.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeExecutionInScopeActivityAddParentScope()
	  public virtual void testVariableAtScopeExecutionInScopeActivityAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_BOUNDARY_TASK);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(SUBPROCESS_CONCURRENT_BOUNDARY_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask1").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree scopeExecution = executionTreeBeforeMigration.Executions[0];

		runtimeService.setVariableLocal(scopeExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtTask()
	  public virtual void testVariableAtTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Task task = taskService.createTaskQuery().singleResult();
		taskService.setVariableLocal(task.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, beforeMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtTaskAddParentScope()
	  public virtual void testVariableAtTaskAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.setVariableLocal(task.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		ExecutionTree userTask1ExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0];

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1ExecutionAfter.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtTaskAndConcurrentExecutionAddParentScope()
	  public virtual void testVariableAtTaskAndConcurrentExecutionAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		Task task = taskService.createTaskQuery().taskDefinitionKey("userTask1").singleResult();
		taskService.setVariableLocal(task.Id, "foo", 42);
		runtimeService.setVariableLocal(task.ExecutionId, "foo", 52);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance taskVarBeforeMigration = testHelper.snapshotBeforeMigration.getSingleTaskVariable(task.Id, "foo");

		ExecutionTree userTask1ExecutionAfter = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0];

		Assert.assertEquals(2, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(taskVarBeforeMigration, userTask1ExecutionAfter.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeExecutionBecomeNonScope()
	  public virtual void testVariableAtScopeExecutionBecomeNonScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_BOUNDARY_TASK);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree scopeExecution = executionTreeBeforeMigration.Executions[0];

		runtimeService.setVariableLocal(scopeExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, processInstance.Id);

		// and the variable is concurrent local, i.e. expands on tree expansion
		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("userTask").execute();

		VariableInstance variableAfterExpansion = runtimeService.createVariableInstanceQuery().singleResult();
		Assert.assertNotNull(variableAfterExpansion);
		Assert.assertNotSame(processInstance.Id, variableAfterExpansion.ExecutionId);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionBecomeScope()
	  public virtual void testVariableAtConcurrentExecutionBecomeScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree concurrentExecution = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];

		runtimeService.setVariableLocal(concurrentExecution.Id, "foo", 42);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		VariableInstance beforeMigration = testHelper.snapshotBeforeMigration.getSingleVariable("foo");
		ExecutionTree userTask1CCExecution = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask1")[0].Parent;

		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);
		testHelper.assertVariableMigratedToExecution(beforeMigration, userTask1CCExecution.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentAndScopeExecutionBecomeNonScope()
	  public virtual void testVariableAtConcurrentAndScopeExecutionBecomeNonScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(CONCURRENT_BOUNDARY_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree scopeExecution = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];
		ExecutionTree concurrentExecution = scopeExecution.Parent;

		runtimeService.setVariableLocal(scopeExecution.Id, "foo", 42);
		runtimeService.setVariableLocal(concurrentExecution.Id, "foo", 42);

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("expected exception");
		}
		catch (ProcessEngineException e)
		{
		  Assert.assertThat(e.Message, CoreMatchers.containsString("The variable 'foo' exists in both, this scope" + " and concurrent local in the parent scope. Migrating to a non-scope activity would overwrite one of them."));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtParentScopeExecutionAndScopeExecutionBecomeNonScope()
	  public virtual void testVariableAtParentScopeExecutionAndScopeExecutionBecomeNonScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ONE_BOUNDARY_TASK);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree scopeExecution = executionTreeBeforeMigration.getLeafExecutions("userTask")[0];

		runtimeService.setVariableLocal(scopeExecution.Id, "foo", "userTaskScopeValue");
		runtimeService.setVariableLocal(processInstance.Id, "foo", "processScopeValue");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the process scope variable was overwritten due to a compacted execution tree
		Assert.assertEquals(1, testHelper.snapshotAfterMigration.getVariables().Count);

		VariableInstance variable = testHelper.snapshotAfterMigration.getVariables().GetEnumerator().next();

		Assert.assertEquals("userTaskScopeValue", variable.Value);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionAddParentScopeBecomeNonConcurrent()
	  public virtual void testVariableAtConcurrentExecutionAddParentScopeBecomeNonConcurrent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "subProcessValue").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree task1CcExecution = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];
		ExecutionTree task2CcExecution = executionTreeBeforeMigration.getLeafExecutions("userTask2")[0];

		runtimeService.setVariableLocal(task1CcExecution.Id, "foo", "task1Value");
		runtimeService.setVariableLocal(task2CcExecution.Id, "foo", "task2Value");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the io mapping variable was overwritten due to a compacted execution tree
		Assert.assertEquals(2, testHelper.snapshotAfterMigration.getVariables().Count);

		IList<string> values = new List<string>();
		foreach (VariableInstance variable in testHelper.snapshotAfterMigration.getVariables())
		{
		  values.Add((string) variable.Value);
		}

		Assert.assertTrue(values.Contains("task1Value"));
		Assert.assertTrue(values.Contains("task2Value"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddScopeWithInputMappingAndVariableOnConcurrentExecutions()
	  public virtual void testAddScopeWithInputMappingAndVariableOnConcurrentExecutions()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "inputOutputValue").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];
		ExecutionTree userTask2CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask2")[0];

		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", "customValue");
		runtimeService.setVariableLocal(userTask2CCExecutionBefore.Id, "foo", "customValue");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the scope variable instance has been overwritten during compaction (conform to prior behavior);
		// although this is tested here, changing this behavior may be ok in the future
		ICollection<VariableInstance> variables = testHelper.snapshotAfterMigration.getVariables();
		Assert.assertEquals(2, variables.Count);

		foreach (VariableInstance variable in variables)
		{
		  Assert.assertEquals("customValue", variable.Value);
		}

		ExecutionTree subProcessExecution = testHelper.snapshotAfterMigration.ExecutionTree.getLeafExecutions("userTask2")[0].Parent;

		Assert.assertNotNull(testHelper.snapshotAfterMigration.getSingleVariable(subProcessExecution.Id, "foo"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeAndConcurrentExecutionAddParentScope()
	  public virtual void testVariableAtScopeAndConcurrentExecutionAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];
		ExecutionTree userTask2CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask2")[0];

		runtimeService.setVariableLocal(processInstance.Id, "foo", "processInstanceValue");
		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", "task1Value");
		runtimeService.setVariableLocal(userTask2CCExecutionBefore.Id, "foo", "task2Value");

		VariableInstance processScopeVariable = runtimeService.createVariableInstanceQuery().variableValueEquals("foo", "processInstanceValue").singleResult();
		VariableInstance task1Variable = runtimeService.createVariableInstanceQuery().variableValueEquals("foo", "task1Value").singleResult();
		VariableInstance task2Variable = runtimeService.createVariableInstanceQuery().variableValueEquals("foo", "task2Value").singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the scope variable instance has been overwritten during compaction (conform to prior behavior);
		// although this is tested here, changing this behavior may be ok in the future
		Assert.assertEquals(3, testHelper.snapshotAfterMigration.getVariables().Count);

		VariableInstance processScopeVariableAfterMigration = testHelper.snapshotAfterMigration.getVariable(processScopeVariable.Id);
		Assert.assertNotNull(processScopeVariableAfterMigration);
		Assert.assertEquals("processInstanceValue", processScopeVariableAfterMigration.Value);

		VariableInstance task1VariableAfterMigration = testHelper.snapshotAfterMigration.getVariable(task1Variable.Id);
		Assert.assertNotNull(task1VariableAfterMigration);
		Assert.assertEquals("task1Value", task1VariableAfterMigration.Value);

		VariableInstance task2VariableAfterMigration = testHelper.snapshotAfterMigration.getVariable(task2Variable.Id);
		Assert.assertNotNull(task2VariableAfterMigration);
		Assert.assertEquals("task2Value", task2VariableAfterMigration.Value);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtScopeAndConcurrentExecutionRemoveParentScope()
	  public virtual void testVariableAtScopeAndConcurrentExecutionRemoveParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = runtimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExecutionTree executionTreeBeforeMigration = ExecutionTree.forExecution(processInstance.Id, rule.ProcessEngine);

		ExecutionTree userTask1CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask1")[0];
		ExecutionTree userTask2CCExecutionBefore = executionTreeBeforeMigration.getLeafExecutions("userTask2")[0];
		ExecutionTree subProcessExecution = userTask1CCExecutionBefore.Parent;

		runtimeService.setVariableLocal(subProcessExecution.Id, "foo", "subProcessValue");
		runtimeService.setVariableLocal(userTask1CCExecutionBefore.Id, "foo", "task1Value");
		runtimeService.setVariableLocal(userTask2CCExecutionBefore.Id, "foo", "task2Value");

		VariableInstance task1Variable = runtimeService.createVariableInstanceQuery().variableValueEquals("foo", "task1Value").singleResult();
		VariableInstance task2Variable = runtimeService.createVariableInstanceQuery().variableValueEquals("foo", "task2Value").singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the scope variable instance has been overwritten during compaction (conform to prior behavior);
		// although this is tested here, changing this behavior may be ok in the future
		ICollection<VariableInstance> variables = testHelper.snapshotAfterMigration.getVariables();
		Assert.assertEquals(2, variables.Count);

		VariableInstance task1VariableAfterMigration = testHelper.snapshotAfterMigration.getVariable(task1Variable.Id);
		Assert.assertNotNull(task1VariableAfterMigration);
		Assert.assertEquals("task1Value", task1VariableAfterMigration.Value);

		VariableInstance task2VariableAfterMigration = testHelper.snapshotAfterMigration.getVariable(task2Variable.Id);
		Assert.assertNotNull(task2VariableAfterMigration);
		Assert.assertEquals("task2Value", task2VariableAfterMigration.Value);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInTransition()
	  public virtual void testVariableAtConcurrentExecutionInTransition()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask").startBeforeActivity("userTask").execute();

		Execution concurrentExecution = runtimeService.createExecutionQuery().activityId("userTask").list().get(0);
		Job jobForExecution = rule.ManagementService.createJobQuery().executionId(concurrentExecution.Id).singleResult();

		runtimeService.setVariableLocal(concurrentExecution.Id, "var", "value");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Job jobAfterMigration = rule.ManagementService.createJobQuery().jobId(jobForExecution.Id).singleResult();

		testHelper.assertVariableMigratedToExecution(testHelper.snapshotBeforeMigration.getSingleVariable("var"), jobAfterMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testVariableAtConcurrentExecutionInTransitionAddParentScope()
	  public virtual void testVariableAtConcurrentExecutionInTransitionAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask").startBeforeActivity("userTask").execute();

		Execution concurrentExecution = runtimeService.createExecutionQuery().activityId("userTask").list().get(0);
		Job jobForExecution = rule.ManagementService.createJobQuery().executionId(concurrentExecution.Id).singleResult();

		runtimeService.setVariableLocal(concurrentExecution.Id, "var", "value");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Job jobAfterMigration = rule.ManagementService.createJobQuery().jobId(jobForExecution.Id).singleResult();

		testHelper.assertVariableMigratedToExecution(testHelper.snapshotBeforeMigration.getSingleVariable("var"), jobAfterMigration.ExecutionId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCanMigrateWithObjectVariableThatFailsOnDeserialization()
	  public virtual void testCanMigrateWithObjectVariableThatFailsOnDeserialization()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ObjectValue objectValue = Variables.serializedObjectValue("does/not/deserialize").serializationDataFormat(Variables.SerializationDataFormats.JAVA).objectTypeName("and.this.is.a.nonexisting.Class").create();

		runtimeService.setVariable(processInstance.Id, "var", objectValue);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		ObjectValue migratedValue = runtimeService.getVariableTyped(processInstance.Id, "var", false);
		Assert.assertEquals(objectValue.ValueSerialized, migratedValue.ValueSerialized);
		Assert.assertEquals(objectValue.ObjectTypeName, migratedValue.ObjectTypeName);
	  }

	}

}