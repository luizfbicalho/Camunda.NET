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
//	import static org.camunda.bpm.engine.test.util.ActivityInstanceAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.util.ExecutionAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.fail;


	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using TimerCatchIntermediateEventJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerCatchIntermediateEventJobHandler;
	using TimerExecuteNestedActivityJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerExecuteNestedActivityJobHandler;
	using TimerStartEventSubprocessJobHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerStartEventSubprocessJobHandler;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using MigrationPlan = org.camunda.bpm.engine.migration.MigrationPlan;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using EventSubscription = org.camunda.bpm.engine.runtime.EventSubscription;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
	using ActivityInstanceAssertThatClause = org.camunda.bpm.engine.test.util.ActivityInstanceAssert.ActivityInstanceAssertThatClause;
	using ExecutionAssert = org.camunda.bpm.engine.test.util.ExecutionAssert;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using Assert = org.junit.Assert;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class MigrationTestRule : ProcessEngineTestRule
	{

	  public ProcessInstanceSnapshot snapshotBeforeMigration;
	  public ProcessInstanceSnapshot snapshotAfterMigration;

	  public MigrationTestRule(ProcessEngineRule processEngineRule) : base(processEngineRule)
	  {
	  }

	  public virtual string getSingleExecutionIdForActivity(ActivityInstance activityInstance, string activityId)
	  {
		ActivityInstance singleInstance = getSingleActivityInstance(activityInstance, activityId);

		string[] executionIds = singleInstance.ExecutionIds;
		if (executionIds.Length == 1)
		{
		  return executionIds[0];
		}
		else
		{
		  throw new Exception("There is more than one execution assigned to activity instance " + singleInstance.Id);
		}
	  }

	  public virtual string getSingleExecutionIdForActivityBeforeMigration(string activityId)
	  {
		return getSingleExecutionIdForActivity(snapshotBeforeMigration.ActivityTree, activityId);
	  }

	  public virtual string getSingleExecutionIdForActivityAfterMigration(string activityId)
	  {
		return getSingleExecutionIdForActivity(snapshotAfterMigration.ActivityTree, activityId);
	  }

	  public virtual ActivityInstance getSingleActivityInstance(ActivityInstance tree, string activityId)
	  {
		ActivityInstance[] activityInstances = tree.getActivityInstances(activityId);
		if (activityInstances.Length == 1)
		{
		  return activityInstances[0];
		}
		else
		{
		  throw new Exception("There is not exactly one activity instance for activity " + activityId);
		}
	  }

	  public virtual ActivityInstance getSingleActivityInstanceBeforeMigration(string activityId)
	  {
		return getSingleActivityInstance(snapshotBeforeMigration.ActivityTree, activityId);
	  }

	  public virtual ActivityInstance getSingleActivityInstanceAfterMigration(string activityId)
	  {
		return getSingleActivityInstance(snapshotAfterMigration.ActivityTree, activityId);
	  }

	  public virtual ProcessInstanceSnapshot takeFullProcessInstanceSnapshot(ProcessInstance processInstance)
	  {
		return takeProcessInstanceSnapshot(processInstance).full();
	  }

	  public virtual ProcessInstanceSnapshotBuilder takeProcessInstanceSnapshot(ProcessInstance processInstance)
	  {
		return new ProcessInstanceSnapshotBuilder(processInstance, processEngine);
	  }

	  public virtual ProcessInstance createProcessInstanceAndMigrate(MigrationPlan migrationPlan)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId);

		migrateProcessInstance(migrationPlan, processInstance);
		return processInstance;
	  }

	  public virtual ProcessInstance createProcessInstanceAndMigrate(MigrationPlan migrationPlan, IDictionary<string, object> variables)
	  {
		ProcessInstance processInstance = processEngine.RuntimeService.startProcessInstanceById(migrationPlan.SourceProcessDefinitionId, variables);

		migrateProcessInstance(migrationPlan, processInstance);
		return processInstance;
	  }

	  public virtual void migrateProcessInstance(MigrationPlan migrationPlan, ProcessInstance processInstance)
	  {
		snapshotBeforeMigration = takeFullProcessInstanceSnapshot(processInstance);

		RuntimeService runtimeService = processEngine.RuntimeService;

		runtimeService.newMigration(migrationPlan).processInstanceIds(Collections.singletonList(snapshotBeforeMigration.ProcessInstanceId)).execute();

		// fetch updated process instance
		processInstance = runtimeService.createProcessInstanceQuery().processInstanceId(processInstance.Id).singleResult();

		snapshotAfterMigration = takeFullProcessInstanceSnapshot(processInstance);
	  }

	  public virtual void triggerTimer()
	  {
		Job job = assertTimerJobExists(snapshotAfterMigration);
		processEngine.ManagementService.executeJob(job.Id);
	  }

	  public virtual ExecutionAssert assertExecutionTreeAfterMigration()
	  {
		return assertThat(snapshotAfterMigration.ExecutionTree);
	  }

	  public virtual ActivityInstanceAssertThatClause assertActivityTreeAfterMigration()
	  {
		return assertThat(snapshotAfterMigration.ActivityTree);
	  }

	  public virtual void assertEventSubscriptionsMigrated(string activityIdBefore, string activityIdAfter, string eventName)
	  {
		IList<EventSubscription> eventSubscriptionsBefore = snapshotBeforeMigration.getEventSubscriptionsForActivityIdAndEventName(activityIdAfter, eventName);

		foreach (EventSubscription eventSubscription in eventSubscriptionsBefore)
		{
		  assertEventSubscriptionMigrated(eventSubscription, activityIdAfter, eventName);
		}
	  }

	  protected internal virtual void assertEventSubscriptionMigrated(EventSubscription eventSubscriptionBefore, string activityIdAfter, string eventName)
	  {
		EventSubscription eventSubscriptionAfter = snapshotAfterMigration.getEventSubscriptionById(eventSubscriptionBefore.Id);
		assertNotNull("Expected that an event subscription with id '" + eventSubscriptionBefore.Id + "' " + "exists after migration", eventSubscriptionAfter);

		assertEquals(eventSubscriptionBefore.EventType, eventSubscriptionAfter.EventType);
		assertEquals(activityIdAfter, eventSubscriptionAfter.ActivityId);
		assertEquals(eventName, eventSubscriptionAfter.EventName);
	  }


	  public virtual void assertEventSubscriptionMigrated(string activityIdBefore, string activityIdAfter, string eventName)
	  {
		EventSubscription eventSubscriptionBefore = snapshotBeforeMigration.getEventSubscriptionForActivityIdAndEventName(activityIdBefore, eventName);
		assertNotNull("Expected that an event subscription for activity '" + activityIdBefore + "' exists before migration", eventSubscriptionBefore);

		assertEventSubscriptionMigrated(eventSubscriptionBefore, activityIdAfter, eventName);
	  }

	  public virtual void assertEventSubscriptionMigrated(string activityIdBefore, string eventNameBefore, string activityIdAfter, string eventNameAfter)
	  {
		EventSubscription eventSubscriptionBefore = snapshotBeforeMigration.getEventSubscriptionForActivityIdAndEventName(activityIdBefore, eventNameBefore);
		assertNotNull("Expected that an event subscription for activity '" + activityIdBefore + "' exists before migration", eventSubscriptionBefore);

		assertEventSubscriptionMigrated(eventSubscriptionBefore, activityIdAfter, eventNameAfter);
	  }

	  public virtual void assertEventSubscriptionRemoved(string activityId, string eventName)
	  {
		EventSubscription eventSubscriptionBefore = snapshotBeforeMigration.getEventSubscriptionForActivityIdAndEventName(activityId, eventName);
		assertNotNull("Expected an event subscription for activity '" + activityId + "' before the migration", eventSubscriptionBefore);

		foreach (EventSubscription eventSubscription in snapshotAfterMigration.EventSubscriptions)
		{
		  if (eventSubscriptionBefore.Id.Equals(eventSubscription.Id))
		  {
			fail("Expected event subscription '" + eventSubscriptionBefore.Id + "' to be removed after migration");
		  }
		}
	  }

	  public virtual void assertEventSubscriptionCreated(string activityId, string eventName)
	  {
		EventSubscription eventSubscriptionAfter = snapshotAfterMigration.getEventSubscriptionForActivityIdAndEventName(activityId, eventName);
		assertNotNull("Expected an event subscription for activity '" + activityId + "' after the migration", eventSubscriptionAfter);

		foreach (EventSubscription eventSubscription in snapshotBeforeMigration.EventSubscriptions)
		{
		  if (eventSubscriptionAfter.Id.Equals(eventSubscription.Id))
		  {
			fail("Expected event subscription '" + eventSubscriptionAfter.Id + "' to be created after migration");
		  }
		}
	  }

	  public virtual void assertTimerJob(Job job)
	  {
		assertEquals("Expected job to be a timer job", TimerEntity.TYPE, ((JobEntity) job).Type);
	  }

	  public virtual Job assertTimerJobExists(ProcessInstanceSnapshot snapshot)
	  {
		IList<Job> jobs = snapshot.Jobs;
		assertEquals(1, jobs.Count);
		Job job = jobs[0];
		assertTimerJob(job);
		return job;
	  }

	  public virtual void assertJobCreated(string activityId, string handlerType)
	  {
		JobDefinition jobDefinitionAfter = snapshotAfterMigration.getJobDefinitionForActivityIdAndType(activityId, handlerType);
		assertNotNull("Expected that a job definition for activity '" + activityId + "' exists after migration", jobDefinitionAfter);

		Job jobAfter = snapshotAfterMigration.getJobForDefinitionId(jobDefinitionAfter.Id);
		assertNotNull("Expected that a job for activity '" + activityId + "' exists after migration", jobAfter);
		assertTimerJob(jobAfter);
		assertEquals(jobDefinitionAfter.ProcessDefinitionId, jobAfter.ProcessDefinitionId);
		assertEquals(jobDefinitionAfter.ProcessDefinitionKey, jobAfter.ProcessDefinitionKey);

		foreach (Job job in snapshotBeforeMigration.Jobs)
		{
		  if (jobAfter.Id.Equals(job.Id))
		  {
			fail("Expected job '" + jobAfter.Id + "' to be created first after migration");
		  }
		}
	  }

	  public virtual void assertJobRemoved(string activityId, string handlerType)
	  {
		JobDefinition jobDefinitionBefore = snapshotBeforeMigration.getJobDefinitionForActivityIdAndType(activityId, handlerType);
		assertNotNull("Expected that a job definition for activity '" + activityId + "' exists before migration", jobDefinitionBefore);

		Job jobBefore = snapshotBeforeMigration.getJobForDefinitionId(jobDefinitionBefore.Id);
		assertNotNull("Expected that a job for activity '" + activityId + "' exists before migration", jobBefore);
		assertTimerJob(jobBefore);

		foreach (Job job in snapshotAfterMigration.Jobs)
		{
		  if (jobBefore.Id.Equals(job.Id))
		  {
			fail("Expected job '" + jobBefore.Id + "' to be removed after migration");
		  }
		}
	  }

	  public virtual void assertJobMigrated(string activityIdBefore, string activityIdAfter, string handlerType)
	  {
		JobDefinition jobDefinitionBefore = snapshotBeforeMigration.getJobDefinitionForActivityIdAndType(activityIdBefore, handlerType);
		assertNotNull("Expected that a job definition for activity '" + activityIdBefore + "' exists before migration", jobDefinitionBefore);

		Job jobBefore = snapshotBeforeMigration.getJobForDefinitionId(jobDefinitionBefore.Id);
		assertNotNull("Expected that a timer job for activity '" + activityIdBefore + "' exists before migration", jobBefore);

		assertJobMigrated(jobBefore, activityIdAfter, jobBefore.Duedate);
	  }

	  public virtual void assertJobMigrated(Job jobBefore, string activityIdAfter)
	  {
		assertJobMigrated(jobBefore, activityIdAfter, jobBefore.Duedate);
	  }

	  public virtual void assertJobMigrated(Job jobBefore, string activityIdAfter, DateTime dueDateAfter)
	  {

		Job jobAfter = snapshotAfterMigration.getJobById(jobBefore.Id);
		assertNotNull("Expected that a job with id '" + jobBefore.Id + "' exists after migration", jobAfter);

		JobDefinition jobDefinitionAfter = snapshotAfterMigration.getJobDefinitionForActivityIdAndType(activityIdAfter, ((JobEntity) jobBefore).JobHandlerType);
		assertNotNull("Expected that a job definition for activity '" + activityIdAfter + "' exists after migration", jobDefinitionAfter);

		assertEquals(jobBefore.Id, jobAfter.Id);
		assertEquals("Expected that job is assigned to job definition '" + jobDefinitionAfter.Id + "' after migration", jobDefinitionAfter.Id, jobAfter.JobDefinitionId);
		assertEquals("Expected that job is assigned to deployment '" + snapshotAfterMigration.DeploymentId + "' after migration", snapshotAfterMigration.DeploymentId, jobAfter.DeploymentId);
		assertEquals(dueDateAfter, jobAfter.Duedate);
		assertEquals(((JobEntity) jobBefore).Type, ((JobEntity) jobAfter).Type);
		assertEquals(jobBefore.Priority, jobAfter.Priority);
		assertEquals(jobDefinitionAfter.ProcessDefinitionId, jobAfter.ProcessDefinitionId);
		assertEquals(jobDefinitionAfter.ProcessDefinitionKey, jobAfter.ProcessDefinitionKey);
	  }

	  public virtual void assertBoundaryTimerJobCreated(string activityId)
	  {
		assertJobCreated(activityId, TimerExecuteNestedActivityJobHandler.TYPE);
	  }

	  public virtual void assertBoundaryTimerJobRemoved(string activityId)
	  {
		assertJobRemoved(activityId, TimerExecuteNestedActivityJobHandler.TYPE);
	  }

	  public virtual void assertBoundaryTimerJobMigrated(string activityIdBefore, string activityIdAfter)
	  {
		assertJobMigrated(activityIdBefore, activityIdAfter, TimerExecuteNestedActivityJobHandler.TYPE);
	  }

	  public virtual void assertIntermediateTimerJobCreated(string activityId)
	  {
		assertJobCreated(activityId, TimerCatchIntermediateEventJobHandler.TYPE);
	  }

	  public virtual void assertIntermediateTimerJobRemoved(string activityId)
	  {
		assertJobRemoved(activityId, TimerCatchIntermediateEventJobHandler.TYPE);
	  }

	  public virtual void assertIntermediateTimerJobMigrated(string activityIdBefore, string activityIdAfter)
	  {
		assertJobMigrated(activityIdBefore, activityIdAfter, TimerCatchIntermediateEventJobHandler.TYPE);
	  }

	  public virtual void assertEventSubProcessTimerJobCreated(string activityId)
	  {
		assertJobCreated(activityId, TimerStartEventSubprocessJobHandler.TYPE);
	  }

	  public virtual void assertEventSubProcessTimerJobRemoved(string activityId)
	  {
		assertJobRemoved(activityId, TimerStartEventSubprocessJobHandler.TYPE);
	  }

	  public virtual void assertVariableMigratedToExecution(VariableInstance variableBefore, string executionId)
	  {
		assertVariableMigratedToExecution(variableBefore, executionId, variableBefore.ActivityInstanceId);
	  }

	  public virtual void assertVariableMigratedToExecution(VariableInstance variableBefore, string executionId, string activityInstanceId)
	  {
		VariableInstance variableAfter = snapshotAfterMigration.getVariable(variableBefore.Id);

		Assert.assertNotNull("Variable with id " + variableBefore.Id + " does not exist", variableAfter);

		Assert.assertEquals(activityInstanceId, variableAfter.ActivityInstanceId);
		Assert.assertEquals(variableBefore.CaseExecutionId, variableAfter.CaseExecutionId);
		Assert.assertEquals(variableBefore.CaseInstanceId, variableAfter.CaseInstanceId);
		Assert.assertEquals(variableBefore.ErrorMessage, variableAfter.ErrorMessage);
		Assert.assertEquals(executionId, variableAfter.ExecutionId);
		Assert.assertEquals(variableBefore.Id, variableAfter.Id);
		Assert.assertEquals(variableBefore.Name, variableAfter.Name);
		Assert.assertEquals(variableBefore.ProcessInstanceId, variableAfter.ProcessInstanceId);
		Assert.assertEquals(variableBefore.TaskId, variableAfter.TaskId);
		Assert.assertEquals(variableBefore.TenantId, variableAfter.TenantId);
		Assert.assertEquals(variableBefore.TypeName, variableAfter.TypeName);
		Assert.assertEquals(variableBefore.Value, variableAfter.Value);
	  }

	  public virtual void assertSuperExecutionOfCaseInstance(string caseInstanceId, string expectedSuperExecutionId)
	  {
		CaseExecutionEntity calledInstance = (CaseExecutionEntity) processEngine.CaseService.createCaseInstanceQuery().caseInstanceId(caseInstanceId).singleResult();

		Assert.assertEquals(expectedSuperExecutionId, calledInstance.SuperExecutionId);
	  }

	  public virtual void assertSuperExecutionOfProcessInstance(string processInstance, string expectedSuperExecutionId)
	  {
		ExecutionEntity calledInstance = (ExecutionEntity) processEngine.RuntimeService.createProcessInstanceQuery().processInstanceId(processInstance).singleResult();

		Assert.assertEquals(expectedSuperExecutionId, calledInstance.SuperExecutionId);
	  }

	}

}