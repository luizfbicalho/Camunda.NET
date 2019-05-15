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
//	import static org.camunda.bpm.engine.test.util.MigratingProcessInstanceValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using AsyncContinuationJobHandler = org.camunda.bpm.engine.impl.jobexecutor.AsyncContinuationJobHandler;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using TransitionInstance = org.camunda.bpm.engine.runtime.TransitionInstance;
	using AlwaysFailingDelegate = org.camunda.bpm.engine.test.api.mgmt.AlwaysFailingDelegate;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.migration.ModifiableBpmnModelInstance.modify;
	using AsyncProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.AsyncProcessModels;
	using EventSubProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.EventSubProcessModels;
	using MultiInstanceProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.MultiInstanceProcessModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using BpmnModelInstance = org.camunda.bpm.model.bpmn.BpmnModelInstance;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Ignore = org.junit.Ignore;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationTransitionInstancesTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationTransitionInstancesTest()
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
//ORIGINAL LINE: @After public void resetClock()
	  public virtual void resetClock()
	  {
		ClockUtil.reset();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstance()
	  public virtual void testMigrateAsyncBeforeTransitionInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree("userTask").scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).transition("userTask").done());

		testHelper.assertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceChangeActivityId()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).changeElementId("userTask", "userTaskReplacement"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTaskReplacement").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("userTask", "userTaskReplacement", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTaskReplacement");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceConcurrent()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceConcurrent()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(migrationPlan.SourceProcessDefinitionId).startBeforeActivity("userTask").startBeforeActivity("userTask").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		TransitionInstance[] transitionInstances = testHelper.snapshotAfterMigration.ActivityTree.getTransitionInstances("userTask");

		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").concurrent().noScope().id(transitionInstances[0].ExecutionId).up().child("userTask").concurrent().noScope().id(transitionInstances[1].ExecutionId).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).transition("userTask").transition("userTask").done());

		Assert.assertEquals(2, testHelper.snapshotAfterMigration.Jobs.Count);

		// and it is possible to successfully execute the migrated job
		foreach (Job job in testHelper.snapshotAfterMigration.Jobs)
		{
		  rule.ManagementService.executeJob(job.Id);
		  testHelper.completeTask("userTask");
		}

		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstance()
	  public virtual void testMigrateAsyncAfterTransitionInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceChangeActivityId()
	  public virtual void testMigrateAsyncAfterTransitionInstanceChangeActivityId()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).changeElementId("userTask1", "userTaskReplacement1").changeElementId("userTask2", "userTaskReplacement2"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTaskReplacement1").mapActivities("userTask2", "userTaskReplacement2").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTaskReplacement1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTaskReplacement2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceRemoveIncomingFlow()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceRemoveIncomingFlow()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).removeFlowNode("startEvent"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceAddIncomingFlow()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceAddIncomingFlow()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).removeFlowNode("startEvent"));
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("userTask").execute();

		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase1()
	  public virtual void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).removeFlowNode("endEvent").removeFlowNode("userTask2"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase2()
	  public virtual void testMigrateAsyncAfterTransitionInstanceRemoveOutgoingFlowCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_SUBPROCESS_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask1").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase1()
	  public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).removeFlowNode("endEvent").removeFlowNode("userTask2"));
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the process instance
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase2()
	  public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).activityBuilder("userTask1").userTask("userTask3").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);


		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the process instance
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase3()
	  public virtual void testMigrateAsyncAfterTransitionInstanceAddOutgoingFlowCase3()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).changeElementId("flow1", "flow2").activityBuilder("userTask1").sequenceFlowId("flow3").userTask("userTask3").endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask1", "Transition instance is assigned to a sequence flow that cannot be matched in the target activity");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterTransitionInstanceReplaceOutgoingFlow()
	  public virtual void testMigrateAsyncAfterTransitionInstanceReplaceOutgoingFlow()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS).changeElementId("flow1", "flow2"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);
		testHelper.completeTask("userTask1");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("userTask1", "userTask1", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask2");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateTransitionInstanceJobProperties()
	  public virtual void testMigrateTransitionInstanceJobProperties()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Job jobBeforeMigration = rule.ManagementService.createJobQuery().singleResult();
		rule.ManagementService.setJobPriority(jobBeforeMigration.Id, 42);

		// TODO: fix CAM-5692
	//    Date newDueDate = new DateTime().plusHours(10).toDate();
	//    rule.getManagementService().setJobDuedate(jobBeforeMigration.getId(), newDueDate);
		rule.ManagementService.setJobRetries(jobBeforeMigration.Id, 52);
		rule.ManagementService.suspendJobById(jobBeforeMigration.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Job job = testHelper.snapshotAfterMigration.Jobs[0];

		Assert.assertEquals(42, job.Priority);
	//    Assert.assertEquals(newDueDate, job.getDuedate());
		Assert.assertEquals(52, job.Retries);
		Assert.assertTrue(job.Suspended);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeStartEventTransitionInstanceCase1()
	  public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase1()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("startEvent", "startEvent").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("startEvent", "startEvent", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		Assert.assertEquals("Replace this non-API assert with a proper test case that fails when the wrong atomic operation is used", "process-start", ((JobEntity) job).JobHandlerConfigurationRaw);
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeStartEventTransitionInstanceCase2()
	  public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase2()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("startEvent", "subProcessStart").build();

		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("startEvent", "A transition instance that instantiates the process can only be migrated to a process-level flow node");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeStartEventTransitionInstanceCase3()
	  public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase3()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcessStart", "subProcessStart").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("subProcessStart", "subProcessStart", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeStartEventTransitionInstanceCase4()
	  public virtual void testMigrateAsyncBeforeStartEventTransitionInstanceCase4()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_START_EVENT_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_START_EVENT_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcessStart", "startEvent").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("subProcessStart", "startEvent", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceAddParentScope()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("userTask").scope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").transition("userTask").done());

		testHelper.assertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);

		// and it is possible to successfully execute the migrated job
		Job job = testHelper.snapshotAfterMigration.Jobs[0];
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceConcurrentAddParentScope()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceConcurrentAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_SUBPROCESS_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(migrationPlan.SourceProcessDefinitionId).startBeforeActivity("userTask").startBeforeActivity("userTask").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child(null).scope().child("userTask").concurrent().noScope().up().child("userTask").concurrent().noScope().done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).beginScope("subProcess").transition("userTask").transition("userTask").done());

		Assert.assertEquals(2, testHelper.snapshotAfterMigration.Jobs.Count);

		// and it is possible to successfully execute the migrated job
		foreach (Job job in testHelper.snapshotAfterMigration.Jobs)
		{
		  rule.ManagementService.executeJob(job.Id);
		  testHelper.completeTask("userTask");
		}

		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceWithIncident()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceWithIncident()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).changeElementId("userTask", "newUserTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask", "newUserTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Job job = rule.ManagementService.createJobQuery().singleResult();
		rule.ManagementService.setJobRetries(job.Id, 0);

		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		Incident incidentAfterMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		assertNotNull(incidentAfterMigration);
		// and it is still the same incident
		assertEquals(incidentBeforeMigration.Id, incidentAfterMigration.Id);
		assertEquals(job.Id, incidentAfterMigration.Configuration);

		// and the activity and process definition references were updated
		assertEquals("newUserTask", incidentAfterMigration.ActivityId);
		assertEquals(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);

		// and it is possible to successfully execute the migrated job
		rule.ManagementService.executeJob(job.Id);

		// and complete the task and process instance
		testHelper.completeTask("newUserTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }




//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeInnerMultiInstance()
	  public virtual void testMigrateAsyncBeforeInnerMultiInstance()
	  {
		// given
		BpmnModelInstance model = modify(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS).asyncBeforeInnerMiActivity("userTask");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		IList<Job> jobs = testHelper.snapshotAfterMigration.Jobs;
		Assert.assertEquals(3, jobs.Count);

		testHelper.assertJobMigrated(jobs[0], "userTask");
		testHelper.assertJobMigrated(jobs[1], "userTask");
		testHelper.assertJobMigrated(jobs[2], "userTask");

		// and it is possible to successfully execute the migrated jobs
		foreach (Job job in jobs)
		{
		  rule.ManagementService.executeJob(job.Id);
		}

		// and complete the task and process instance
		testHelper.completeAnyTask("userTask");
		testHelper.completeAnyTask("userTask");
		testHelper.completeAnyTask("userTask");
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterInnerMultiInstance()
	  public virtual void testMigrateAsyncAfterInnerMultiInstance()
	  {
		// given
		BpmnModelInstance model = modify(MultiInstanceProcessModels.PAR_MI_ONE_TASK_PROCESS).asyncAfterInnerMiActivity("userTask");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		testHelper.completeAnyTask("userTask");
		testHelper.completeAnyTask("userTask");
		testHelper.completeAnyTask("userTask");

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		IList<Job> jobs = testHelper.snapshotAfterMigration.Jobs;
		Assert.assertEquals(3, jobs.Count);

		testHelper.assertJobMigrated(jobs[0], "userTask");
		testHelper.assertJobMigrated(jobs[1], "userTask");
		testHelper.assertJobMigrated(jobs[2], "userTask");

		// and it is possible to successfully execute the migrated jobs
		foreach (Job job in jobs)
		{
		  rule.ManagementService.executeJob(job.Id);
		}

		// and complete the process instance
		testHelper.assertProcessEnded(testHelper.snapshotBeforeMigration.ProcessInstanceId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateAsyncBeforeTransitionInstanceToNonAsyncActivity()
	  public virtual void testCannotMigrateAsyncBeforeTransitionInstanceToNonAsyncActivity()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask", "Target activity is not asyncBefore");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateAsyncAfterTransitionInstanceToNonAsyncActivity()
	  public virtual void testCannotMigrateAsyncAfterTransitionInstanceToNonAsyncActivity()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_AFTER_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.ONE_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("userTask1", "userTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		testHelper.completeTask("userTask1");

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask1", "Target activity is not asyncAfter");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateUnmappedTransitionInstance()
	  public virtual void testCannotMigrateUnmappedTransitionInstance()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).build();

		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask", "There is no migration instruction for this instance's activity");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateUnmappedTransitionInstanceAtNonLeafActivity()
	  public virtual void testCannotMigrateUnmappedTransitionInstanceAtNonLeafActivity()
	  {
		// given
		BpmnModelInstance model = modify(ProcessModels.SUBPROCESS_PROCESS).activityBuilder("subProcess").camundaAsyncBefore(true).done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).build();

		// when
		try
		{
		  testHelper.createProcessInstanceAndMigrate(migrationPlan);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("subProcess", "There is no migration instruction for this instance's activity");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCannotMigrateUnmappedTransitionInstanceWithIncident()
	  public virtual void testCannotMigrateUnmappedTransitionInstanceWithIncident()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);

		// the user task is not mapped in the migration plan, i.e. there is no instruction to migrate the job
		// and the incident
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		Job job = rule.ManagementService.createJobQuery().singleResult();
		rule.ManagementService.setJobRetries(job.Id, 0);

		// when
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasTransitionInstanceFailures("userTask", "There is no migration instruction for this instance's activity");
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncBeforeTransitionInstanceToDifferentProcessKey()
	  public virtual void testMigrateAsyncBeforeTransitionInstanceToDifferentProcessKey()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(AsyncProcessModels.ASYNC_BEFORE_USER_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		// when
		testHelper.createProcessInstanceAndMigrate(migrationPlan);

		// then
		testHelper.assertJobMigrated("userTask", "userTask", AsyncContinuationJobHandler.TYPE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMigrateAsyncAfterCompensateEventSubProcessStartEvent()
	  public virtual void testMigrateAsyncAfterCompensateEventSubProcessStartEvent()
	  {
		// given
		BpmnModelInstance model = modify(EventSubProcessModels.COMPENSATE_EVENT_SUBPROCESS_PROCESS).flowNodeBuilder("eventSubProcessStart").camundaAsyncAfter().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(model);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("subProcess", "subProcess").mapActivities("eventSubProcess", "eventSubProcess").mapActivities("eventSubProcessStart", "eventSubProcessStart").build();

		ProcessInstance processInstance = rule.RuntimeService.createProcessInstanceById(sourceProcessDefinition.Id).startBeforeActivity("eventSubProcess").execute();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("eventSubProcessStart", "eventSubProcessStart", AsyncContinuationJobHandler.TYPE);
	  }

	  /// <summary>
	  /// Does not apply since asyncAfter cannot be used with boundary events
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore @Test public void testMigrateAsyncAfterBoundaryEventWithChangedEventScope()
	  public virtual void testMigrateAsyncAfterBoundaryEventWithChangedEventScope()
	  {
		BpmnModelInstance sourceProcess = modify(ProcessModels.PARALLEL_GATEWAY_PROCESS).activityBuilder("userTask1").boundaryEvent("boundary").message("Message").camundaAsyncAfter().userTask("afterBoundaryTask").endEvent().done();
		BpmnModelInstance targetProcess = modify(sourceProcess).swapElementIds("userTask1", "userTask2");

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(sourceProcess);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(targetProcess);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("boundary", "boundary").mapActivities("userTask1", "userTask1").mapActivities("userTask2", "userTask2").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then
		testHelper.assertJobMigrated("boundary", "boundary", AsyncContinuationJobHandler.TYPE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailMigrateFailedJobIncident()
	  public virtual void testFailMigrateFailedJobIncident()
	  {
		// given
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		BpmnModelInstance model = ProcessModels.newModel().startEvent().serviceTask("serviceTask").camundaAsyncBefore().camundaClass(typeof(AlwaysFailingDelegate).FullName).endEvent().done();

		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(model);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(model).changeElementId("serviceTask", "newServiceTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		string processInstanceId = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id).Id;
		testHelper.executeAvailableJobs();

		// when
		try
		{
		  rule.RuntimeService.newMigration(migrationPlan).processInstanceIds(processInstanceId).execute();

		  Assert.fail("should fail");
		}
		catch (MigratingProcessInstanceValidationException e)
		{
		  // then
		  Assert.assertTrue(e is MigratingProcessInstanceValidationException);
		}
	  }

	}

}