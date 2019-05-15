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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{

	using DateUtils = org.apache.commons.lang3.time.DateUtils;
	using Batch = org.camunda.bpm.engine.batch.Batch;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using ProcessInstanceQuery = org.camunda.bpm.engine.runtime.ProcessInstanceQuery;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class UserOperationLogJobTest : AbstractUserOperationLogTest
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetJobPriority()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I set a job priority
		managementService.setJobPriority(job.Id, 42);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY).singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertEquals(job.Id, userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, userOperationLogEntry.OperationType);

		assertEquals("priority", userOperationLogEntry.Property);
		assertEquals("42", userOperationLogEntry.NewValue);
		assertEquals("0", userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetRetries()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I set the job retries
		managementService.setJobRetries(job.Id, 4);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES).singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertEquals(job.Id, userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("retries", userOperationLogEntry.Property);
		assertEquals("4", userOperationLogEntry.NewValue);
		assertEquals("3", userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetRetriesByJobDefinitionId()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I set the job retries
		managementService.setJobRetriesByJobDefinitionId(job.JobDefinitionId, 4);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES).singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("retries", userOperationLogEntry.Property);
		assertEquals("4", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetRetriesAsync()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I set the job retries
		Batch batch = managementService.setJobRetriesAsync(Arrays.asList(job.Id), 4);

		// then three op log entries are written
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES);
		assertEquals(3, query.count());

		// check 'retries' entry
		UserOperationLogEntry userOperationLogEntry = query.property("retries").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("retries", userOperationLogEntry.Property);
		assertEquals("4", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		// check 'nrOfInstances' entry
		userOperationLogEntry = query.property("nrOfInstances").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("nrOfInstances", userOperationLogEntry.Property);
		assertEquals("1", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		// check 'async' entry
		userOperationLogEntry = query.property("async").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("async", userOperationLogEntry.Property);
		assertEquals("true", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		managementService.deleteBatch(batch.Id, true);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetRetriesAsyncProcessInstanceId()
	  {
		// given a job
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I set the job retries
		Batch batch = managementService.setJobRetriesAsync(Arrays.asList(processInstance.Id), (ProcessInstanceQuery) null, 4);

		// then three op log entries are written
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES);
		assertEquals(3, query.count());

		// check 'retries' entry
		UserOperationLogEntry userOperationLogEntry = query.property("retries").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("retries", userOperationLogEntry.Property);
		assertEquals("4", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		// check 'nrOfInstances' entry
		userOperationLogEntry = query.property("nrOfInstances").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("nrOfInstances", userOperationLogEntry.Property);
		assertEquals("1", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		// check 'async' entry
		userOperationLogEntry = query.property("async").singleResult();
		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertNull(userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES, userOperationLogEntry.OperationType);

		assertEquals("async", userOperationLogEntry.Property);
		assertEquals("true", userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertNull(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertNull(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertNull(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertNull(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertNull(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		managementService.deleteBatch(batch.Id, true);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetJobDueDate()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// and set the job due date
		DateTime newDate = new DateTime(ClockUtil.CurrentTime.Ticks + 2 * 1000);
		managementService.setJobDuedate(job.Id, newDate);

		// then one op log entry is written
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_DUEDATE);
		assertEquals(1, query.count());

		// assert details
		UserOperationLogEntry entry = query.singleResult();
		assertEquals(job.Id, entry.JobId);
		assertEquals(job.DeploymentId, entry.DeploymentId);
		assertEquals(job.JobDefinitionId, entry.JobDefinitionId);
		assertEquals("duedate", entry.Property);
		assertNull(entry.OrgValue);
		assertEquals(newDate, new DateTime(Convert.ToInt64(entry.NewValue)));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/bpmn/event/timer/TimerRecalculationTest.testFinishedJob.bpmn20.xml"})]
	  public virtual void testRecalculateJobDueDate()
	  {
		// given a job
		Dictionary<string, object> variables1 = new Dictionary<string, object>();
		DateTime duedate = ClockUtil.CurrentTime;
		variables1["dueDate"] = duedate;

		runtimeService.startProcessInstanceByKey("intermediateTimerEventExample", variables1);
		Job job = managementService.createJobQuery().singleResult();

		// when I recalculate the job due date
		managementService.recalculateJobDuedate(job.Id, false);

		// then one op log entry is written
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_RECALC_DUEDATE);
		assertEquals(2, query.count());

		// assert details
		UserOperationLogEntry entry = query.property("duedate").singleResult();
		assertEquals(job.Id, entry.JobId);
		assertEquals(job.DeploymentId, entry.DeploymentId);
		assertEquals(job.JobDefinitionId, entry.JobDefinitionId);
		assertEquals("duedate", entry.Property);
		assertTrue(DateUtils.truncatedEquals(duedate, new DateTime(Convert.ToInt64(entry.OrgValue)), DateTime.SECOND));
		assertTrue(DateUtils.truncatedEquals(duedate, new DateTime(Convert.ToInt64(entry.NewValue)), DateTime.SECOND));

		entry = query.property("creationDateBased").singleResult();
		assertEquals(job.Id, entry.JobId);
		assertEquals(job.DeploymentId, entry.DeploymentId);
		assertEquals(job.JobDefinitionId, entry.JobDefinitionId);
		assertEquals("creationDateBased", entry.Property);
		assertNull(entry.OrgValue);
		assertFalse(Convert.ToBoolean(entry.NewValue));
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testDelete()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I delete a job
		managementService.deleteJob(job.Id);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE).singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertEquals(job.Id, userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE, userOperationLogEntry.OperationType);

		assertNull(userOperationLogEntry.Property);
		assertNull(userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testExecute()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		Job job = managementService.createJobQuery().singleResult();

		// when I execute a job manually
		managementService.executeJob(job.Id);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EXECUTE).singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB, userOperationLogEntry.EntityType);
		assertEquals(job.Id, userOperationLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EXECUTE, userOperationLogEntry.OperationType);

		assertNull(userOperationLogEntry.Property);
		assertNull(userOperationLogEntry.NewValue);
		assertNull(userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(job.JobDefinitionId, userOperationLogEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, userOperationLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testExecuteByJobExecutor()
	  {
		// given a job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		assertEquals(1L, managementService.createJobQuery().count());

		// when a job is executed by the job executor
		waitForJobExecutorToProcessAllJobs(TimeUnit.MILLISECONDS.convert(5L, TimeUnit.SECONDS));

		// then no op log entry is written
		assertEquals(0L, managementService.createJobQuery().count());
		long logEntriesCount = historyService.createUserOperationLogQuery().operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_EXECUTE).count();
		assertEquals(0L, logEntriesCount);
	  }
	}

}