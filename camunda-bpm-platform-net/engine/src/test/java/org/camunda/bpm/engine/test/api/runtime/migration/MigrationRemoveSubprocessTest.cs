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
//	import static org.junit.Assert.assertEquals;


	using ExecutionListener = org.camunda.bpm.engine.@delegate.ExecutionListener;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using DelegateEvent = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateEvent;
	using DelegateExecutionListener = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateExecutionListener;
	using MigrationPlanValidationReportAssert = org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationRemoveSubprocessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationRemoveSubprocessTest()
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
//ORIGINAL LINE: @Test public void testRemoveScopeForNonScopeActivity()
	  public virtual void testRemoveScopeForNonScopeActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveScopeForScopeActivity()
	  public virtual void testRemoveScopeForScopeActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveScopeForConcurrentNonScopeActivity()
	  public virtual void testRemoveScopeForConcurrentNonScopeActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask1").concurrent().noScope().up().child("userTask2").concurrent().noScope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

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
//ORIGINAL LINE: @Test public void testRemoveScopeForConcurrentScopeActivity()
	  public virtual void testRemoveScopeForConcurrentScopeActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS_SUB_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).concurrent().noScope().child("userTask1").scope().up().up().child(null).concurrent().noScope().child("userTask2").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

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
//ORIGINAL LINE: @Test public void testRemoveConcurrentScope()
	  public virtual void testRemoveConcurrentScope()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask").mapActivities("userTask2", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

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
//ORIGINAL LINE: @Test public void testRemoveConcurrentScope2()
	  public virtual void testRemoveConcurrentScope2()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask2").concurrent().noScope().up().child(null).concurrent().noScope().child("userTask1").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).beginScope("subProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess1").Id).activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

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
//ORIGINAL LINE: @Test @Ignore("Missing feature CAM-5407") public void testRemoveScopeAndMoveToConcurrentActivity()
	  public virtual void testRemoveScopeAndMoveToConcurrentActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask2").concurrent().noScope().up().child(null).concurrent().noScope().child("userTask1").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).beginScope("subProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

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

	  /// <summary>
	  /// Remove when implementing CAM-5407
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotRemoveScopeAndMoveToConcurrentActivity()
	  public virtual void testCannotRemoveScopeAndMoveToConcurrentActivity()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS);

		// when
		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		  Assert.fail("should not validate");
		}
		catch (MigrationPlanValidationException e)
		{
		  MigrationPlanValidationReportAssert.assertThat(e.ValidationReport).hasInstructionFailures("userTask2", "The closest mapped ancestor 'subProcess' is mapped to scope 'subProcess' which is not an ancestor of target scope 'userTask2'");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveMultipleScopes()
	  public virtual void testRemoveMultipleScopes()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());


		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEndListenerInvocationForRemovedScope()
	  public virtual void testEndListenerInvocationForRemovedScope()
	  {
		// given
		DelegateEvent.clearEvents();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(DelegateExecutionListener).FullName).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(1, recordedEvents.Count);

		DelegateEvent @event = recordedEvents[0];
		assertEquals(sourceProcessDefinition.Id, @event.ProcessDefinitionId);
		assertEquals("subProcess", @event.CurrentActivityId);
		assertEquals(testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id, @event.ActivityInstanceId);

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipListenerInvocationForRemovedScope()
	  public virtual void testSkipListenerInvocationForRemovedScope()
	  {
		// given
		DelegateEvent.clearEvents();

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_END, typeof(DelegateExecutionListener).FullName).done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).skipCustomListeners().execute();

		// then
		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(0, recordedEvents.Count);

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIoMappingInvocationForRemovedScope()
	  public virtual void testIoMappingInvocationForRemovedScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaOutputParameter("foo", "bar").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();

		// then
		VariableInstance inputVariable = rule.RuntimeService.createVariableInstanceQuery().singleResult();
		Assert.assertNotNull(inputVariable);
		assertEquals("foo", inputVariable.Name);
		assertEquals("bar", inputVariable.Value);
		assertEquals(processInstance.Id, inputVariable.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipIoMappingInvocationForRemovedScope()
	  public virtual void testSkipIoMappingInvocationForRemovedScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaOutputParameter("foo", "bar").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).skipIoMappings().execute();

		// then
		assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().count());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotRemoveParentScopeAndMoveOutOfGrandParentScope()
	  public virtual void testCannotRemoveParentScopeAndMoveOutOfGrandParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TRIPLE_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TRIPLE_SUBPROCESS_PROCESS);

		// when
		try
		{
		  // subProcess2 is not migrated
		  // subProcess 3 is moved out of the subProcess1 scope (by becoming a subProcess1 itself)
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess1", "subProcess1").mapActivities("subProcess3", "subProcess1").mapActivities("userTask", "userTask").build();

		  Assert.fail("should not validate");
		}
		catch (MigrationPlanValidationException e)
		{
		  MigrationPlanValidationReportAssert.assertThat(e.ValidationReport).hasInstructionFailures("subProcess3", "The closest mapped ancestor 'subProcess1' is mapped to scope 'subProcess1' which is not an ancestor of target scope 'subProcess1'");
		}
	  }

	}

}