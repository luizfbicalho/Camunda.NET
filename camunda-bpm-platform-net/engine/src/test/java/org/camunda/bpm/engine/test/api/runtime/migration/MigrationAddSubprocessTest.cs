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
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using DelegateEvent = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateEvent;
	using DelegateExecutionListener = org.camunda.bpm.engine.test.bpmn.multiinstance.DelegateExecutionListener;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Variables = org.camunda.bpm.engine.variable.Variables;
	using ParallelGatewayBuilder = org.camunda.bpm.model.bpmn.builder.ParallelGatewayBuilder;
	using UserTask = org.camunda.bpm.model.bpmn.instance.UserTask;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationAddSubprocessTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationAddSubprocessTest()
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
//ORIGINAL LINE: @Test public void testScopeUserTaskMigration()
	  public virtual void testScopeUserTaskMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SCOPE_TASK_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testConcurrentScopeUserTaskMigration()
	  public virtual void testConcurrentScopeUserTaskMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS_SUB_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child(null).concurrent().noScope().child("userTask1").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask1")).up().up().child(null).concurrent().noScope().child("userTask2").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("userTask2")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		assertEquals(2, migratedTasks.Count);

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
//ORIGINAL LINE: @Test public void testUserTaskMigration()
	  public virtual void testUserTaskMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

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
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask1").concurrent().noScope().up().child("userTask2").concurrent().noScope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		assertEquals(2, migratedTasks.Count);

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
//ORIGINAL LINE: @Test public void testConcurrentThreeUserTaskMigration()
	  public virtual void testConcurrentThreeUserTaskMigration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).getBuilderForElementById("fork", typeof(ParallelGatewayBuilder)).userTask("userTask3").endEvent().done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS).getBuilderForElementById("fork", typeof(ParallelGatewayBuilder)).userTask("userTask3").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask2").mapActivities("userTask2", "userTask3").mapActivities("userTask3", "userTask1").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask1").concurrent().noScope().up().child("userTask2").concurrent().noScope().up().child("userTask3").concurrent().noScope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask3").Id).activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).activity("userTask3", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		assertEquals(3, migratedTasks.Count);

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
//ORIGINAL LINE: @Test public void testNestedScopesMigration1()
	  public virtual void testNestedScopesMigration1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("subProcess", "outerSubProcess").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("subProcess")).child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("outerSubProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).beginScope("innerSubProcess").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testNestedScopesMigration2()
	  public virtual void testNestedScopesMigration2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("subProcess", "innerSubProcess").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("subProcess")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("outerSubProcess").beginScope("innerSubProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMultipleInstancesOfScope()
	  public virtual void testMultipleInstancesOfScope()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").mapActivities("subProcess", "outerSubProcess").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("subProcess").startBeforeActivity("subProcess").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).concurrent().noScope().child(null).scope().child("userTask").scope().up().up().up().child(null).concurrent().noScope().child(null).scope().child("userTask").scope().done());

		ActivityInstance activityInstance = testHelper.snapshotBeforeMigration.ActivityTree;
		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("outerSubProcess", activityInstance.getActivityInstances("subProcess")[0].Id).beginScope("innerSubProcess").activity("userTask", activityInstance.getActivityInstances("subProcess")[0].getActivityInstances("userTask")[0].Id).endScope().endScope().beginScope("outerSubProcess", activityInstance.getActivityInstances("subProcess")[1].Id).beginScope("innerSubProcess").activity("userTask", activityInstance.getActivityInstances("subProcess")[1].getActivityInstances("userTask")[0].Id).done());

		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		assertEquals(2, migratedTasks.Count);

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
//ORIGINAL LINE: @Test public void testChangeActivityId()
	  public virtual void testChangeActivityId()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask2").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask1").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask2").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask2");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeScopeActivityId()
	  public virtual void testChangeScopeActivityId()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS_SUB_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask2").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask1").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask2").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask2");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListenerInvocationForNewlyCreatedScope()
	  public virtual void testListenerInvocationForNewlyCreatedScope()
	  {
		// given
		DelegateEvent.clearEvents();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		IList<DelegateEvent> recordedEvents = DelegateEvent.Events;
		assertEquals(1, recordedEvents.Count);

		DelegateEvent @event = recordedEvents[0];
		assertEquals(targetProcessDefinition.Id, @event.ProcessDefinitionId);
		assertEquals("subProcess", @event.CurrentActivityId);

		DelegateEvent.clearEvents();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipListenerInvocationForNewlyCreatedScope()
	  public virtual void testSkipListenerInvocationForNewlyCreatedScope()
	  {
		// given
		DelegateEvent.clearEvents();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaExecutionListenerClass(org.camunda.bpm.engine.@delegate.ExecutionListener_Fields.EVENTNAME_START, typeof(DelegateExecutionListener).FullName).done());

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
//ORIGINAL LINE: @Test public void testIoMappingInvocationForNewlyCreatedScope()
	  public virtual void testIoMappingInvocationForNewlyCreatedScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "bar").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).execute();


		// then
		VariableInstance inputVariable = rule.RuntimeService.createVariableInstanceQuery().singleResult();
		Assert.assertNotNull(inputVariable);
		assertEquals("foo", inputVariable.Name);
		assertEquals("bar", inputVariable.Value);

		ActivityInstance activityInstance = rule.RuntimeService.getActivityInstance(processInstance.Id);
		assertEquals(activityInstance.getActivityInstances("subProcess")[0].Id, inputVariable.ActivityInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSkipIoMappingInvocationForNewlyCreatedScope()
	  public virtual void testSkipIoMappingInvocationForNewlyCreatedScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaInputParameter("foo", "bar").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);
		rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(Arrays.asList(processInstance.Id)).skipIoMappings().execute();

		// then
		assertEquals(0, rule.RuntimeService.createVariableInstanceQuery().count());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteMigratedInstance()
	  public virtual void testDeleteMigratedInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_SCOPE_TASKS_SUB_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then it is possible to delete the process instance
		string processInstanceId = testHelper.snapshotBeforeMigration.ProcessInstanceId;
		rule.RuntimeService.deleteProcessInstance(processInstanceId, null);
		testHelper.assertProcessEnded(processInstanceId);
	  }

	  /// <summary>
	  /// Readd when we implement migration for multi-instance
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Ignore public void testAddParentScopeToMultiInstance()
	  public virtual void testAddParentScopeToMultiInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.ONE_TASK_PROCESS).getModelElementById<UserTask>("userTask").builder().multiInstance().parallel().camundaCollection("collectionVar").camundaElementVariable("elementVar").done());
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).getModelElementById<UserTask>("userTask").builder().multiInstance().parallel().camundaCollection("collectionVar").camundaElementVariable("elementVar").done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask#multiInstanceBody", "userTask#multiInstanceBody").mapActivities("userTask", "userTask").build();

		IList<string> miElements = new List<string>();
		miElements.Add("a");
		miElements.Add("b");
		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id, Variables.createVariables().putValue("collectionVar", miElements));

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").beginMiBody("userTask").activity("userTask").activity("userTask").activity("userTask").done());

		// the element variables still exist
		IList<Task> migratedTasks = testHelper.snapshotAfterMigration.Tasks;
		assertEquals(2, migratedTasks.Count);

		IList<string> collectedElementsVars = new List<string>();
		foreach (Task migratedTask in migratedTasks)
		{
		  collectedElementsVars.Add((string) rule.TaskService.getVariable(migratedTask.Id, "elementVar"));
		}

		Assert.assertTrue(collectedElementsVars.Contains("a"));
		Assert.assertTrue(collectedElementsVars.Contains("b"));

		// and it is possible to successfully complete the migrated instance
		foreach (Task migratedTask in migratedTasks)
		{
		  rule.TaskService.complete(migratedTask.Id);
		}

		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTwoScopes()
	  public virtual void testAddTwoScopes()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("outerSubProcess").beginScope("innerSubProcess").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);
		assertEquals(targetProcessDefinition.Id, migratedTask.ProcessDefinitionId);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddTwoConcurrentScopes()
	  public virtual void testAddTwoConcurrentScopes()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.DOUBLE_PARALLEL_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then there is only one instance of outerSubProcess
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child(null).concurrent().noScope().child("userTask1").scope().up().up().child(null).concurrent().noScope().child("userTask2").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("outerSubProcess").beginScope("innerSubProcess1").activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).endScope().beginScope("innerSubProcess2").activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).done());

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
//ORIGINAL LINE: @Test public void testCanMigrateParentScopeWayTooHigh()
	  public virtual void testCanMigrateParentScopeWayTooHigh()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.TRIPLE_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess1").mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then there is only one instance of outerSubProcess
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("subProcess")).child(null).scope().child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess1", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).beginScope("subProcess2").beginScope("subProcess3").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Task migratedTask = testHelper.snapshotAfterMigration.getTaskForKey("userTask");
		Assert.assertNotNull(migratedTask);

		// and it is possible to successfully complete the migrated instance
		rule.TaskService.complete(migratedTask.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMoveConcurrentActivityIntoSiblingScope()
	  public virtual void testMoveConcurrentActivityIntoSiblingScope()
	  {

		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_TASK_AND_SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.PARALLEL_GATEWAY_SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).concurrent().noScope().child("userTask2").scope().up().up().child(null).concurrent().noScope().child("userTask1").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask2", testHelper.getSingleActivityInstanceBeforeMigration("userTask2").Id).endScope().beginScope("subProcess", testHelper.getSingleActivityInstanceBeforeMigration("subProcess").Id).activity("userTask1", testHelper.getSingleActivityInstanceBeforeMigration("userTask1").Id).done());

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
//ORIGINAL LINE: @Test public void testAddScopeDoesNotBecomeAsync()
	  public virtual void testAddScopeDoesNotBecomeAsync()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaAsyncBefore().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then the async flag for the subprocess was not relevant for instantiation
		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").activity("userTask", testHelper.getSingleActivityInstanceBeforeMigration("userTask").Id).done());

		Assert.assertEquals(0, testHelper.snapshotAfterMigration.Jobs.Count);
	  }

	}

}