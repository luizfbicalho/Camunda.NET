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
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;

	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using GatewayModels = org.camunda.bpm.engine.test.api.runtime.migration.models.GatewayModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationGatewayTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationGatewayTest()
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
//ORIGINAL LINE: @Test public void testParallelGatewayContinueExecution()
	  public virtual void testParallelGatewayContinueExecution()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		testHelper.completeTask("parallel1");
		testHelper.completeTask("afterJoin");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelGatewayAssertTrees()
	  public virtual void testParallelGatewayAssertTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("parallel1").noScope().concurrent().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("parallel1")).up().child("join").noScope().concurrent().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("join")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("parallel1", testHelper.getSingleActivityInstanceBeforeMigration("parallel1").Id).activity("join", testHelper.getSingleActivityInstanceBeforeMigration("join").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testParallelGatewayAddScope()
	  public virtual void testParallelGatewayAddScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW_IN_SUBPROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		testHelper.completeTask("parallel1");
		testHelper.completeTask("afterJoin");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInclusiveGatewayContinueExecution()
	  public virtual void testInclusiveGatewayContinueExecution()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		testHelper.completeTask("parallel1");
		testHelper.completeTask("afterJoin");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInclusiveGatewayAssertTrees()
	  public virtual void testInclusiveGatewayAssertTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("parallel1").noScope().concurrent().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("parallel1")).up().child("join").noScope().concurrent().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("join")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("parallel1", testHelper.getSingleActivityInstanceBeforeMigration("parallel1").Id).activity("join", testHelper.getSingleActivityInstanceBeforeMigration("join").Id).done());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInclusiveGatewayAddScope()
	  public virtual void testInclusiveGatewayAddScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW_IN_SUBPROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		testHelper.completeTask("parallel1");
		testHelper.completeTask("afterJoin");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateParallelToInclusiveGateway()
	  public virtual void testCannotMigrateParallelToInclusiveGateway()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("join", "join").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("join", "Activities have incompatible types " + "(ParallelGatewayActivityBehavior is not compatible with InclusiveGatewayActivityBehavior)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateInclusiveToParallelGateway()
	  public virtual void testCannotMigrateInclusiveToParallelGateway()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.INCLUSIVE_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("join", "join").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("join", "Activities have incompatible types " + "(InclusiveGatewayActivityBehavior is not compatible with ParallelGatewayActivityBehavior)");
		}
	  }

	  /// <summary>
	  /// Ensures that situations are avoided in which more tokens end up at the target gateway
	  /// than it has incoming flows
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotRemoveGatewayIncomingSequenceFlow()
	  public virtual void testCannotRemoveGatewayIncomingSequenceFlow()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(GatewayModels.PARALLEL_GW).removeFlowNode("parallel2"));

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("join", "join").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("join", "The target gateway must have at least the same number of incoming sequence flows that the source gateway has");
		}
	  }

	  /// <summary>
	  /// Ensures that situations are avoided in which more tokens end up at the target gateway
	  /// than it has incoming flows
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddGatewayIncomingSequenceFlow()
	  public virtual void testAddGatewayIncomingSequenceFlow()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(GatewayModels.PARALLEL_GW).flowNodeBuilder("fork").userTask("parallel3").connectTo("join").done());

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("parallel2");

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("parallel1", "parallel1").mapActivities("join", "join").build();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Assert.assertEquals(1, rule.TaskService.createTaskQuery().count());
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		rule.RuntimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("join").execute();
		Assert.assertEquals(0, rule.TaskService.createTaskQuery().taskDefinitionKey("afterJoin").count());

		testHelper.completeTask("parallel1");
		testHelper.completeTask("afterJoin");
		testHelper.assertProcessEnded(processInstance.Id);
	  }

	  /// <summary>
	  /// Ensures that situations are avoided in which more tokens end up at the target gateway
	  /// than it has incoming flows
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotRemoveParentScope()
	  public virtual void testCannotRemoveParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW_IN_SUBPROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("join", "join").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("join", "The gateway's flow scope 'subProcess' must be mapped");
		}
	  }

	  /// <summary>
	  /// Ensures that situations are avoided in which more tokens end up at the target gateway
	  /// than it has incoming flows
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMapMultipleGatewaysToOne()
	  public virtual void testCannotMapMultipleGatewaysToOne()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(GatewayModels.PARALLEL_GW);

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("join", "join").mapActivities("fork", "join").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("join", "Only one gateway can be mapped to gateway 'join'");
		}
	  }
	}


}