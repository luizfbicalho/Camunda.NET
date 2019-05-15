using System;

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

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class UserOperationLogJobDefinitionTest : AbstractUserOperationLogTest
	{

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetOverridingPriority()
	  {
		// given a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// when I set a job priority
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// then an op log entry is written
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().singleResult();
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB_DEFINITION, userOperationLogEntry.EntityType);
		assertEquals(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, userOperationLogEntry.OperationType);

		assertEquals("overridingPriority", userOperationLogEntry.Property);
		assertEquals("42", userOperationLogEntry.NewValue);
		assertEquals(null, userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		assertEquals(jobDefinition.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(jobDefinition.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testOverwriteOverridingPriority()
	  {
		// given a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// with an overriding priority
		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis());
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// when I overwrite that priority
		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 10000);
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 43);

		// then this is accessible via the op log
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().orderByTimestamp().desc().listPage(0, 1).get(0);
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB_DEFINITION, userOperationLogEntry.EntityType);
		assertEquals(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, userOperationLogEntry.OperationType);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		assertEquals("overridingPriority", userOperationLogEntry.Property);
		assertEquals("43", userOperationLogEntry.NewValue);
		assertEquals("42", userOperationLogEntry.OrgValue);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testClearOverridingPriority()
	  {
		// given a job definition
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// with an overriding priority
		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis());
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42);

		// when I clear that priority
		ClockUtil.CurrentTime = new DateTime(DateTimeHelper.CurrentUnixTimeMillis() + 10000);
		managementService.clearOverridingJobPriorityForJobDefinition(jobDefinition.Id);

		// then this is accessible via the op log
		UserOperationLogEntry userOperationLogEntry = historyService.createUserOperationLogQuery().orderByTimestamp().desc().listPage(0, 1).get(0);
		assertNotNull(userOperationLogEntry);

		assertEquals(EntityTypes.JOB_DEFINITION, userOperationLogEntry.EntityType);
		assertEquals(jobDefinition.Id, userOperationLogEntry.JobDefinitionId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, userOperationLogEntry.OperationType);

		assertEquals("overridingPriority", userOperationLogEntry.Property);
		assertNull(userOperationLogEntry.NewValue);
		assertEquals("42", userOperationLogEntry.OrgValue);

		assertEquals(USER_ID, userOperationLogEntry.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, userOperationLogEntry.Category);

		assertEquals(jobDefinition.ProcessDefinitionId, userOperationLogEntry.ProcessDefinitionId);
		assertEquals(jobDefinition.ProcessDefinitionKey, userOperationLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, userOperationLogEntry.DeploymentId);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/asyncTaskProcess.bpmn20.xml"})]
	  public virtual void testSetOverridingPriorityCascadeToJobs()
	  {
		// given a job definition and job
		runtimeService.startProcessInstanceByKey("asyncTaskProcess");
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		Job job = managementService.createJobQuery().singleResult();

		// when I set an overriding priority with cascade=true
		managementService.setOverridingJobPriorityForJobDefinition(jobDefinition.Id, 42, true);

		// then there are three op log entries
		assertEquals(3, historyService.createUserOperationLogQuery().count());

		// (1): One for the process instance start
		UserOperationLogEntry processInstanceStartOpLogEntry = historyService.createUserOperationLogQuery().entityType(EntityTypes.PROCESS_INSTANCE).singleResult();
		assertNotNull(processInstanceStartOpLogEntry);

		// (2): One for the job definition priority
		UserOperationLogEntry jobDefOpLogEntry = historyService.createUserOperationLogQuery().entityType(EntityTypes.JOB_DEFINITION).singleResult();
		assertNotNull(jobDefOpLogEntry);

		// (3): and another one for the job priorities
		UserOperationLogEntry jobOpLogEntry = historyService.createUserOperationLogQuery().entityType(EntityTypes.JOB).singleResult();
		assertNotNull(jobOpLogEntry);

		assertEquals("the two job related entries should be part of the same operation", jobDefOpLogEntry.OperationId, jobOpLogEntry.OperationId);

		assertEquals(EntityTypes.JOB, jobOpLogEntry.EntityType);
		assertNull("id should null because it is a bulk update operation", jobOpLogEntry.JobId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY, jobOpLogEntry.OperationType);

		assertEquals("priority", jobOpLogEntry.Property);
		assertEquals("42", jobOpLogEntry.NewValue);
		assertNull("Original Value should be null because it is not known for bulk operations", jobOpLogEntry.OrgValue);

		assertEquals(USER_ID, jobOpLogEntry.UserId);

		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, jobOpLogEntry.Category);

		// these properties should be there to narrow down the bulk update (like a SQL WHERE clasue)
		assertEquals(job.JobDefinitionId, jobOpLogEntry.JobDefinitionId);
		assertNull("an unspecified set of process instances was affected by the operation", jobOpLogEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionId, jobOpLogEntry.ProcessDefinitionId);
		assertEquals(job.ProcessDefinitionKey, jobOpLogEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, jobOpLogEntry.DeploymentId);
	  }

	}

}