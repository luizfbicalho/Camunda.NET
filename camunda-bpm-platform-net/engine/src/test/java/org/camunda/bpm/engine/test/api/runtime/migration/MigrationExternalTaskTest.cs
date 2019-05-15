using System;
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
//	import static org.camunda.bpm.engine.test.util.MigrationPlanValidationReportAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;

	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using MigratingProcessInstanceValidationException = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationException;
	using MigrationInstruction = org.camunda.bpm.engine.migration.MigrationInstruction;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using MigrationPlanValidationException = org.camunda.bpm.engine.migration.MigrationPlanValidationException;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ExternalTaskModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ExternalTaskModels;
	using ProcessModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ProcessModels;
	using ServiceTaskModels = org.camunda.bpm.engine.test.api.runtime.migration.models.ServiceTaskModels;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Assert = org.junit.Assert;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationExternalTaskTest
	{
		private bool InstanceFieldsInitialized = false;

		public MigrationExternalTaskTest()
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


	  public const string WORKER_ID = "foo";

	  protected internal ProcessEngineRule rule = new ProvidedProcessEngineRule();
	  protected internal MigrationTestRule testHelper;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(rule).around(testHelper);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTrees()
	  public virtual void testTrees()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the execution and activity instance tree are exactly as before migration
		testHelper.assertExecutionTreeAfterMigration().hasProcessDefinitionId(targetProcessDefinition.Id).matches(describeExecutionTree(null).scope().id(testHelper.snapshotBeforeMigration.ProcessInstanceId).child("externalTask").scope().id(testHelper.getSingleExecutionIdForActivityBeforeMigration("externalTask")).done());

		testHelper.assertActivityTreeAfterMigration().hasStructure(describeActivityInstanceTree(targetProcessDefinition.Id).activity("externalTask", testHelper.getSingleActivityInstanceBeforeMigration("externalTask").Id).done());

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testProperties()
	  public virtual void testProperties()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY).changeElementId("externalTask", "newExternalTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "newExternalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExternalTask externalTaskBeforeMigration = rule.ExternalTaskService.createExternalTaskQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then all properties are the same apart from the process reference
		ExternalTask externalTaskAfterMigration = rule.ExternalTaskService.createExternalTaskQuery().singleResult();

		Assert.assertEquals("newExternalTask", externalTaskAfterMigration.ActivityId);
		Assert.assertEquals(targetProcessDefinition.Id, externalTaskAfterMigration.ProcessDefinitionId);
		Assert.assertEquals("new" + ProcessModels.PROCESS_KEY, externalTaskAfterMigration.ProcessDefinitionKey);

		Assert.assertEquals(externalTaskBeforeMigration.Priority, externalTaskAfterMigration.Priority);
		Assert.assertEquals(externalTaskBeforeMigration.ActivityInstanceId, externalTaskAfterMigration.ActivityInstanceId);
		Assert.assertEquals(externalTaskBeforeMigration.ErrorMessage, externalTaskAfterMigration.ErrorMessage);
		Assert.assertEquals(externalTaskBeforeMigration.ExecutionId, externalTaskAfterMigration.ExecutionId);
		Assert.assertEquals(externalTaskBeforeMigration.Id, externalTaskAfterMigration.Id);
		Assert.assertEquals(externalTaskBeforeMigration.LockExpirationTime, externalTaskAfterMigration.LockExpirationTime);
		Assert.assertEquals(processInstance.Id, externalTaskAfterMigration.ProcessInstanceId);
		Assert.assertEquals(externalTaskBeforeMigration.Retries, externalTaskAfterMigration.Retries);
		Assert.assertEquals(externalTaskBeforeMigration.TenantId, externalTaskAfterMigration.TenantId);
		Assert.assertEquals(externalTaskBeforeMigration.TopicName, externalTaskAfterMigration.TopicName);
		Assert.assertEquals(externalTaskBeforeMigration.WorkerId, externalTaskAfterMigration.WorkerId);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testContinueProcess()
	  public virtual void testContinueProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the task
		LockedExternalTask task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		rule.ExternalTaskService.complete(task.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeTaskConfiguration()
	  public virtual void testChangeTaskConfiguration()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).serviceTaskBuilder("externalTask").camundaTopic("new" + ExternalTaskModels.TOPIC).camundaTaskPriority(Convert.ToString(ExternalTaskModels.PRIORITY * 2)).done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the task's topic and priority have not changed
		ExternalTask externalTaskAfterMigration = rule.ExternalTaskService.createExternalTaskQuery().singleResult();
		Assert.assertEquals(ExternalTaskModels.PRIORITY.Value, externalTaskAfterMigration.Priority);
		Assert.assertEquals(ExternalTaskModels.TOPIC, externalTaskAfterMigration.TopicName);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testChangeTaskType()
	  public virtual void testChangeTaskType()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ProcessModels.newModel().startEvent().businessRuleTask("externalBusinessRuleTask").camundaType(ExternalTaskModels.EXTERNAL_TASK_TYPE).camundaTopic(ExternalTaskModels.TOPIC).camundaTaskPriority(ExternalTaskModels.PRIORITY.ToString()).endEvent().done());

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalBusinessRuleTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the task and process can be completed
		LockedExternalTask task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		rule.ExternalTaskService.complete(task.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLockedTaskProperties()
	  public virtual void testLockedTaskProperties()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY).changeElementId("externalTask", "newExternalTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "newExternalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		ExternalTask externalTaskBeforeMigration = rule.ExternalTaskService.createExternalTaskQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the locking properties have not been changed
		ExternalTask externalTaskAfterMigration = rule.ExternalTaskService.createExternalTaskQuery().singleResult();

		Assert.assertEquals(externalTaskBeforeMigration.LockExpirationTime, externalTaskAfterMigration.LockExpirationTime);
		Assert.assertEquals(externalTaskBeforeMigration.WorkerId, externalTaskAfterMigration.WorkerId);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLockedTaskContinueProcess()
	  public virtual void testLockedTaskContinueProcess()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).changeElementId(ProcessModels.PROCESS_KEY, "new" + ProcessModels.PROCESS_KEY).changeElementId("externalTask", "newExternalTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "newExternalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		LockedExternalTask externalTask = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the task and the process
		rule.ExternalTaskService.complete(externalTask.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void cannotMigrateFromExternalToClassDelegateServiceTask()
	  public virtual void cannotMigrateFromExternalToClassDelegateServiceTask()
	  {
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ServiceTaskModels.oneClassDelegateServiceTask("foo.Bar"));

		try
		{
		  rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "serviceTask").build();
		  Assert.fail("exception expected");
		}
		catch (MigrationPlanValidationException e)
		{
		  // then
		  assertThat(e.ValidationReport).hasInstructionFailures("externalTask", "Activities have incompatible types (ExternalTaskActivityBehavior is not compatible with" + " ClassDelegateActivityBehavior)");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAddParentScope()
	  public virtual void testAddParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.SUBPROCESS_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the task
		LockedExternalTask task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		rule.ExternalTaskService.complete(task.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRemoveParentScope()
	  public virtual void testRemoveParentScope()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.SUBPROCESS_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "externalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then it is possible to complete the task
		LockedExternalTask task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		rule.ExternalTaskService.complete(task.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncident()
	  public virtual void testIncident()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).changeElementId("externalTask", "newExternalTask"));

		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapActivities("externalTask", "newExternalTask").build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExternalTask externalTask = rule.ExternalTaskService.createExternalTaskQuery().singleResult();
		rule.ExternalTaskService.setRetries(externalTask.Id, 0);

		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();

		// when
		testHelper.migrateProcessInstance(migrationPlan, processInstance);

		// then the incident has migrated
		Incident incidentAfterMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertNotNull(incidentAfterMigration);

		assertEquals(incidentBeforeMigration.Id, incidentAfterMigration.Id);
		assertEquals(org.camunda.bpm.engine.runtime.Incident_Fields.EXTERNAL_TASK_HANDLER_TYPE, incidentAfterMigration.IncidentType);
		assertEquals(externalTask.Id, incidentAfterMigration.Configuration);

		assertEquals("newExternalTask", incidentAfterMigration.ActivityId);
		assertEquals(targetProcessDefinition.Id, incidentAfterMigration.ProcessDefinitionId);
		assertEquals(externalTask.ExecutionId, incidentAfterMigration.ExecutionId);

		// and it is possible to complete the process
		rule.ExternalTaskService.setRetries(externalTask.Id, 1);

		LockedExternalTask task = fetchAndLockSingleTask(ExternalTaskModels.TOPIC);
		rule.ExternalTaskService.complete(task.Id, WORKER_ID);

		testHelper.assertProcessEnded(processInstance.Id);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIncidentWithoutMapExternalTask()
	  public virtual void testIncidentWithoutMapExternalTask()
	  {
		// given
		ProcessDefinition sourceProcessDefinition = testHelper.deployAndGetDefinition(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS);
		ProcessDefinition targetProcessDefinition = testHelper.deployAndGetDefinition(modify(ExternalTaskModels.ONE_EXTERNAL_TASK_PROCESS).changeElementId("externalTask", "newExternalTask"));

		//external task is not mapped to new external task
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		ProcessInstance processInstance = rule.RuntimeService.startProcessInstanceById(sourceProcessDefinition.Id);

		ExternalTask externalTask = rule.ExternalTaskService.createExternalTaskQuery().singleResult();
		rule.ExternalTaskService.setRetries(externalTask.Id, 0);

		Incident incidentBeforeMigration = rule.RuntimeService.createIncidentQuery().singleResult();
		assertNotNull(incidentBeforeMigration);

		// when migration is executed
		try
		{
		  testHelper.migrateProcessInstance(migrationPlan, processInstance);
		  Assert.fail("Exception expected!");
		}
		catch (Exception ex)
		{
		  Assert.assertTrue(ex is MigratingProcessInstanceValidationException);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn"}) public void testProcessDefinitionWithoutIdField()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn"})]
	  public virtual void testProcessDefinitionWithoutIdField()
	  {
		 // given

		ProcessDefinition sourceProcessDefinition = testHelper.deploy("org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn").DeployedProcessDefinitions[0];
		ProcessDefinition targetProcessDefinition = testHelper.deploy("org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn").DeployedProcessDefinitions[0];

		//external task is not mapped to new external task
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		IList<MigrationInstruction> instructions = migrationPlan.Instructions;
		// test that the messageEventDefinition without an id isn't included
		assertEquals(2, instructions.Count);
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn"}) public void testProcessDefinitionWithIdField()
	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithoutIdTest.bpmn"})]
	  public virtual void testProcessDefinitionWithIdField()
	  {
		 // given

		ProcessDefinition sourceProcessDefinition = testHelper.deploy("org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithIdTest.bpmn").DeployedProcessDefinitions[0];
		ProcessDefinition targetProcessDefinition = testHelper.deploy("org/camunda/bpm/engine/test/api/externaltask/ExternalTaskWithIdTest.bpmn").DeployedProcessDefinitions[0];

		//external task is not mapped to new external task
		MigrationPlan migrationPlan = rule.RuntimeService.createMigrationPlan(sourceProcessDefinition.Id, targetProcessDefinition.Id).mapEqualActivities().build();

		IList<MigrationInstruction> instructions = migrationPlan.Instructions;
		assertEquals(2, instructions.Count);
	  }

	  protected internal virtual LockedExternalTask fetchAndLockSingleTask(string topic)
	  {
		IList<LockedExternalTask> tasks = rule.ExternalTaskService.fetchAndLock(1, WORKER_ID).topic(topic, 1000L).execute();

		Assert.assertEquals(1, tasks.Count);

		return tasks[0];
	  }
	}

}