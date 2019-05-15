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
	using ExternalTask = org.camunda.bpm.engine.externaltask.ExternalTask;
	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using RuntimeServiceImpl = org.camunda.bpm.engine.impl.RuntimeServiceImpl;
	using TaskServiceImpl = org.camunda.bpm.engine.impl.TaskServiceImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerActivateJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerActivateJobDefinitionHandler;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using CaseInstance = org.camunda.bpm.engine.runtime.CaseInstance;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Attachment = org.camunda.bpm.engine.task.Attachment;
	using Task = org.camunda.bpm.engine.task.Task;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.EntityTypes.PROCESS_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.ENTITY_TYPE_ATTACHMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.ENTITY_TYPE_IDENTITY_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.ENTITY_TYPE_TASK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_ATTACHMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_GROUP_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ADD_USER_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_ATTACHMENT;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_GROUP_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_USER_LINK;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_JOB_RETRIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_PRIORITY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_JOB;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_JOB_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.ASSIGNEE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.persistence.entity.TaskEntity.OWNER;

	/// <summary>
	/// @author Danny Gräf
	/// </summary>
	public class UserOperationLogQueryTest : AbstractUserOperationLogTest
	{

	  protected internal const string ONE_TASK_PROCESS = "org/camunda/bpm/engine/test/history/oneTaskProcess.bpmn20.xml";
	  protected internal const string ONE_TASK_CASE = "org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn";
	  protected internal const string ONE_EXTERNAL_TASK_PROCESS = "org/camunda/bpm/engine/test/api/externaltask/oneExternalTaskProcess.bpmn20.xml";

	  private ProcessInstance process;
	  private Task userTask;
	  private Execution execution;
	  private string processTaskId;

	  // normalize timestamps for databases which do not provide millisecond presision.
	  private DateTime today = new DateTime((ClockUtil.CurrentTime.Ticks / 1000) * 1000);
	  private DateTime tomorrow = new DateTime(((ClockUtil.CurrentTime.Ticks + 86400000) / 1000) * 1000);
	  private DateTime yesterday = new DateTime(((ClockUtil.CurrentTime.Ticks - 86400000) / 1000) * 1000);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();

		if (userTask != null)
		{
		  historyService.deleteHistoricTaskInstance(userTask.Id);
		}
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQuery()
	  {
		createLogEntries();

		// expect: all entries can be fetched
		assertEquals(18, query().count());

		// entity type
		assertEquals(11, query().entityType(EntityTypes.TASK).count());
		assertEquals(4, query().entityType(EntityTypes.IDENTITY_LINK).count());
		assertEquals(2, query().entityType(EntityTypes.ATTACHMENT).count());
		assertEquals(1, query().entityType(EntityTypes.PROCESS_INSTANCE).count());
		assertEquals(0, query().entityType("unknown entity type").count());

		// operation type
		assertEquals(2, query().operationType(OPERATION_TYPE_CREATE).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_SET_PRIORITY).count());
		assertEquals(4, query().operationType(OPERATION_TYPE_UPDATE).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_ADD_USER_LINK).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_DELETE_USER_LINK).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_ADD_GROUP_LINK).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_DELETE_GROUP_LINK).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_ADD_ATTACHMENT).count());
		assertEquals(1, query().operationType(OPERATION_TYPE_DELETE_ATTACHMENT).count());

		// category
		assertEquals(17, query().categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER).count());
		assertEquals(1, query().categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).count()); // start process instance

		// process and execution reference
		assertEquals(12, query().processDefinitionId(process.ProcessDefinitionId).count());
		assertEquals(12, query().processInstanceId(process.Id).count());
		assertEquals(11, query().executionId(execution.Id).count());

		// task reference
		assertEquals(11, query().taskId(processTaskId).count());
		assertEquals(6, query().taskId(userTask.Id).count());

		// user reference
		assertEquals(11, query().userId("icke").count()); // not includes the create operation called by the process
		assertEquals(6, query().userId("er").count());

		// operation ID
		UserOperationLogQuery updates = query().operationType(OPERATION_TYPE_UPDATE);
		string updateOperationId = updates.list().get(0).OperationId;
		assertEquals(updates.count(), query().operationId(updateOperationId).count());

		// changed properties
		assertEquals(3, query().property(ASSIGNEE).count());
		assertEquals(2, query().property(OWNER).count());

		// ascending order results by time
		IList<UserOperationLogEntry> ascLog = query().orderByTimestamp().asc().list();
		for (int i = 0; i < 5; i++)
		{
		  assertTrue(yesterday.Ticks <= ascLog[i].Timestamp.Ticks);
		}
		for (int i = 5; i < 13; i++)
		{
		  assertTrue(today.Ticks <= ascLog[i].Timestamp.Ticks);
		}
		for (int i = 13; i < 18; i++)
		{
		  assertTrue(tomorrow.Ticks <= ascLog[i].Timestamp.Ticks);
		}

		// descending order results by time
		IList<UserOperationLogEntry> descLog = query().orderByTimestamp().desc().list();
		for (int i = 0; i < 4; i++)
		{
		  assertTrue(tomorrow.Ticks <= descLog[i].Timestamp.Ticks);
		}
		for (int i = 4; i < 11; i++)
		{
		  assertTrue(today.Ticks <= descLog[i].Timestamp.Ticks);
		}
		for (int i = 11; i < 18; i++)
		{
		  assertTrue(yesterday.Ticks <= descLog[i].Timestamp.Ticks);
		}

		// filter by time, created yesterday
		assertEquals(5, query().beforeTimestamp(today).count());
		// filter by time, created today and before
		assertEquals(13, query().beforeTimestamp(tomorrow).count());
		// filter by time, created today and later
		assertEquals(13, query().afterTimestamp(yesterday).count());
		// filter by time, created tomorrow
		assertEquals(5, query().afterTimestamp(today).count());
		assertEquals(0, query().afterTimestamp(today).beforeTimestamp(yesterday).count());
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryWithBackwardCompatibility()
	  {
		createLogEntries();

		// expect: all entries can be fetched
		assertEquals(18, query().count());

		// entity type
		assertEquals(11, query().entityType(ENTITY_TYPE_TASK).count());
		assertEquals(4, query().entityType(ENTITY_TYPE_IDENTITY_LINK).count());
		assertEquals(2, query().entityType(ENTITY_TYPE_ATTACHMENT).count());
		assertEquals(0, query().entityType("unknown entity type").count());
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessInstanceOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.suspendProcessInstanceById(process.Id);
		runtimeService.activateProcessInstanceById(process.Id);

		runtimeService.deleteProcessInstance(process.Id, "a delete reason");

		// then
		assertEquals(4, query().entityType(PROCESS_INSTANCE).count());

		UserOperationLogEntry deleteEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_DELETE).singleResult();

		assertNotNull(deleteEntry);
		assertEquals(process.Id, deleteEntry.ProcessInstanceId);
		assertNotNull(deleteEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", deleteEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, deleteEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, deleteEntry.Category);

		UserOperationLogEntry suspendEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_SUSPEND).singleResult();

		assertNotNull(suspendEntry);
		assertEquals(process.Id, suspendEntry.ProcessInstanceId);
		assertNotNull(suspendEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendEntry.ProcessDefinitionKey);

		assertEquals("suspensionState", suspendEntry.Property);
		assertEquals("suspended", suspendEntry.NewValue);
		assertNull(suspendEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendEntry.Category);

		UserOperationLogEntry activateEntry = query().entityType(PROCESS_INSTANCE).processInstanceId(process.Id).operationType(OPERATION_TYPE_ACTIVATE).singleResult();

		assertNotNull(activateEntry);
		assertEquals(process.Id, activateEntry.ProcessInstanceId);
		assertNotNull(activateEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, activateEntry.DeploymentId);

		assertEquals("suspensionState", activateEntry.Property);
		assertEquals("active", activateEntry.NewValue);
		assertNull(activateEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateEntry.Category);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessInstanceOperationsByProcessDefinitionId()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionId(process.ProcessDefinitionId);
		runtimeService.activateProcessInstanceByProcessDefinitionId(process.ProcessDefinitionId);

		// then
		assertEquals(3, query().entityType(PROCESS_INSTANCE).count());

		UserOperationLogEntry suspendEntry = query().entityType(PROCESS_INSTANCE).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_SUSPEND).singleResult();

		assertNotNull(suspendEntry);
		assertEquals(process.ProcessDefinitionId, suspendEntry.ProcessDefinitionId);
		assertNull(suspendEntry.ProcessInstanceId);
		assertEquals("oneTaskProcess", suspendEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, suspendEntry.DeploymentId);

		assertEquals("suspensionState", suspendEntry.Property);
		assertEquals("suspended", suspendEntry.NewValue);
		assertNull(suspendEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendEntry.Category);

		UserOperationLogEntry activateEntry = query().entityType(PROCESS_INSTANCE).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_ACTIVATE).singleResult();

		assertNotNull(activateEntry);
		assertNull(activateEntry.ProcessInstanceId);
		assertEquals("oneTaskProcess", activateEntry.ProcessDefinitionKey);
		assertEquals(process.ProcessDefinitionId, activateEntry.ProcessDefinitionId);
		assertEquals(deploymentId, activateEntry.DeploymentId);

		assertEquals("suspensionState", activateEntry.Property);
		assertEquals("active", activateEntry.NewValue);
		assertNull(activateEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateEntry.Category);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessInstanceOperationsByProcessDefinitionKey()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.suspendProcessInstanceByProcessDefinitionKey("oneTaskProcess");
		runtimeService.activateProcessInstanceByProcessDefinitionKey("oneTaskProcess");

		// then
		assertEquals(3, query().entityType(PROCESS_INSTANCE).count());

		UserOperationLogEntry suspendEntry = query().entityType(PROCESS_INSTANCE).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_SUSPEND).singleResult();

		assertNotNull(suspendEntry);
		assertNull(suspendEntry.ProcessInstanceId);
		assertNull(suspendEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendEntry.ProcessDefinitionKey);
		assertNull(suspendEntry.DeploymentId);

		assertEquals("suspensionState", suspendEntry.Property);
		assertEquals("suspended", suspendEntry.NewValue);
		assertNull(suspendEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendEntry.Category);

		UserOperationLogEntry activateEntry = query().entityType(PROCESS_INSTANCE).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_ACTIVATE).singleResult();

		assertNotNull(activateEntry);
		assertNull(activateEntry.ProcessInstanceId);
		assertNull(activateEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateEntry.ProcessDefinitionKey);
		assertNull(activateEntry.DeploymentId);

		assertEquals("suspensionState", activateEntry.Property);
		assertEquals("active", activateEntry.NewValue);
		assertNull(activateEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateEntry.Category);
	  }

	  /// <summary>
	  /// CAM-1930: add assertions for additional op log entries here
	  /// </summary>
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessDefinitionOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		repositoryService.suspendProcessDefinitionById(process.ProcessDefinitionId, true, null);
		repositoryService.activateProcessDefinitionById(process.ProcessDefinitionId, true, null);

		// then
		assertEquals(2, query().entityType(PROCESS_DEFINITION).count());

		// Process Definition Suspension
		UserOperationLogEntry suspendDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION).singleResult();

		assertNotNull(suspendDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, suspendDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendDefinitionEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, suspendDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", suspendDefinitionEntry.Property);
		assertEquals("suspended", suspendDefinitionEntry.NewValue);
		assertNull(suspendDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendDefinitionEntry.Category);

		UserOperationLogEntry activateDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION).singleResult();

		assertNotNull(activateDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, activateDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateDefinitionEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, activateDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", activateDefinitionEntry.Property);
		assertEquals("active", activateDefinitionEntry.NewValue);
		assertNull(activateDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateDefinitionEntry.Category);

	  }

	  /// <summary>
	  /// CAM-1930: add assertions for additional op log entries here
	  /// </summary>
	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessDefinitionOperationsByKey()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		repositoryService.suspendProcessDefinitionByKey("oneTaskProcess", true, null);
		repositoryService.activateProcessDefinitionByKey("oneTaskProcess", true, null);

		// then
		assertEquals(2, query().entityType(PROCESS_DEFINITION).count());

		UserOperationLogEntry suspendDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION).singleResult();

		assertNotNull(suspendDefinitionEntry);
		assertNull(suspendDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", suspendDefinitionEntry.ProcessDefinitionKey);
		assertNull(suspendDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", suspendDefinitionEntry.Property);
		assertEquals("suspended", suspendDefinitionEntry.NewValue);
		assertNull(suspendDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendDefinitionEntry.Category);

		UserOperationLogEntry activateDefinitionEntry = query().entityType(PROCESS_DEFINITION).processDefinitionKey("oneTaskProcess").operationType(OPERATION_TYPE_ACTIVATE_PROCESS_DEFINITION).singleResult();

		assertNotNull(activateDefinitionEntry);
		assertNull(activateDefinitionEntry.ProcessDefinitionId);
		assertEquals("oneTaskProcess", activateDefinitionEntry.ProcessDefinitionKey);
		assertNull(activateDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", activateDefinitionEntry.Property);
		assertEquals("active", activateDefinitionEntry.NewValue);
		assertNull(activateDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateDefinitionEntry.Category);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/history/HistoricJobLogTest.testAsyncContinuation.bpmn20.xml"})]
	  public virtual void testQueryJobOperations()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("process");

		// when
		managementService.suspendJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
		managementService.activateJobDefinitionByProcessDefinitionId(process.ProcessDefinitionId);
		managementService.suspendJobByProcessInstanceId(process.Id);
		managementService.activateJobByProcessInstanceId(process.Id);

		// then
		assertEquals(2, query().entityType(JOB_DEFINITION).count());
		assertEquals(2, query().entityType(JOB).count());

		// active job definition
		UserOperationLogEntry activeJobDefinitionEntry = query().entityType(JOB_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_ACTIVATE_JOB_DEFINITION).singleResult();

		assertNotNull(activeJobDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, activeJobDefinitionEntry.ProcessDefinitionId);
		assertEquals(deploymentId, activeJobDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", activeJobDefinitionEntry.Property);
		assertEquals("active", activeJobDefinitionEntry.NewValue);
		assertNull(activeJobDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activeJobDefinitionEntry.Category);

		// active job
		UserOperationLogEntry activateJobIdEntry = query().entityType(JOB).processInstanceId(process.ProcessInstanceId).operationType(OPERATION_TYPE_ACTIVATE_JOB).singleResult();

		assertNotNull(activateJobIdEntry);
		assertEquals(process.ProcessInstanceId, activateJobIdEntry.ProcessInstanceId);
		assertEquals(deploymentId, activateJobIdEntry.DeploymentId);

		assertEquals("suspensionState", activateJobIdEntry.Property);
		assertEquals("active", activateJobIdEntry.NewValue);
		assertNull(activateJobIdEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, activateJobIdEntry.Category);

		// suspended job definition
		UserOperationLogEntry suspendJobDefinitionEntry = query().entityType(JOB_DEFINITION).processDefinitionId(process.ProcessDefinitionId).operationType(OPERATION_TYPE_SUSPEND_JOB_DEFINITION).singleResult();

		assertNotNull(suspendJobDefinitionEntry);
		assertEquals(process.ProcessDefinitionId, suspendJobDefinitionEntry.ProcessDefinitionId);
		assertEquals(deploymentId, suspendJobDefinitionEntry.DeploymentId);

		assertEquals("suspensionState", suspendJobDefinitionEntry.Property);
		assertEquals("suspended", suspendJobDefinitionEntry.NewValue);
		assertNull(suspendJobDefinitionEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendJobDefinitionEntry.Category);

		// suspended job
		UserOperationLogEntry suspendedJobEntry = query().entityType(JOB).processInstanceId(process.ProcessInstanceId).operationType(OPERATION_TYPE_SUSPEND_JOB).singleResult();

		assertNotNull(suspendedJobEntry);
		assertEquals(process.ProcessInstanceId, suspendedJobEntry.ProcessInstanceId);
		assertEquals(deploymentId, suspendedJobEntry.DeploymentId);

		assertEquals("suspensionState", suspendedJobEntry.Property);
		assertEquals("suspended", suspendedJobEntry.NewValue);
		assertNull(suspendedJobEntry.OrgValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, suspendedJobEntry.Category);
	  }

	  [Deployment(resources : { "org/camunda/bpm/engine/test/bpmn/async/FoxJobRetryCmdTest.testFailedServiceTask.bpmn20.xml" })]
	  public virtual void testQueryJobRetryOperationsById()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("failedServiceTask");
		Job job = managementService.createJobQuery().processInstanceId(process.ProcessInstanceId).singleResult();

		managementService.setJobRetries(job.Id, 10);

		// then
		assertEquals(1, query().entityType(JOB).operationType(OPERATION_TYPE_SET_JOB_RETRIES).count());

		UserOperationLogEntry jobRetryEntry = query().entityType(JOB).jobId(job.Id).operationType(OPERATION_TYPE_SET_JOB_RETRIES).singleResult();

		assertNotNull(jobRetryEntry);
		assertEquals(job.Id, jobRetryEntry.JobId);

		assertEquals("3", jobRetryEntry.OrgValue);
		assertEquals("10", jobRetryEntry.NewValue);
		assertEquals("retries", jobRetryEntry.Property);
		assertEquals(job.JobDefinitionId, jobRetryEntry.JobDefinitionId);
		assertEquals(job.ProcessInstanceId, jobRetryEntry.ProcessInstanceId);
		assertEquals(job.ProcessDefinitionKey, jobRetryEntry.ProcessDefinitionKey);
		assertEquals(job.ProcessDefinitionId, jobRetryEntry.ProcessDefinitionId);
		assertEquals(deploymentId, jobRetryEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, jobRetryEntry.Category);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryJobDefinitionOperationWithDelayedJobDefinition()
	  {
		// given
		// a running process instance
		ProcessInstance process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// with a process definition id
		string processDefinitionId = process.ProcessDefinitionId;

		// ...which will be suspended with the corresponding jobs
		managementService.suspendJobDefinitionByProcessDefinitionId(processDefinitionId, true);

		// one week from now
		ClockUtil.CurrentTime = today;
		long oneWeekFromStartTime = today.Ticks + (7 * 24 * 60 * 60 * 1000);

		// when
		// activate the job definition
		managementService.activateJobDefinitionByProcessDefinitionId(processDefinitionId, false, new DateTime(oneWeekFromStartTime));

		// then
		// there is a user log entry for the activation
		long? jobDefinitionEntryCount = query().entityType(JOB_DEFINITION).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB_DEFINITION).processDefinitionId(processDefinitionId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).count();

		assertEquals(1, jobDefinitionEntryCount.Value);

		// there exists a job for the delayed activation execution
		JobQuery jobQuery = managementService.createJobQuery();

		Job delayedActivationJob = jobQuery.timers().active().singleResult();
		assertNotNull(delayedActivationJob);

		// execute job
		managementService.executeJob(delayedActivationJob.Id);

		jobDefinitionEntryCount = query().entityType(JOB_DEFINITION).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_ACTIVATE_JOB_DEFINITION).processDefinitionId(processDefinitionId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).count();

		assertEquals(1, jobDefinitionEntryCount.Value);

		// Clean up db
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly UserOperationLogQueryTest outerInstance;

		  public CommandAnonymousInnerClass(UserOperationLogQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerActivateJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/repository/ProcessDefinitionSuspensionTest.testWithOneAsyncServiceTask.bpmn"})]
	  public virtual void testQueryProcessDefinitionOperationWithDelayedProcessDefinition()
	  {
		// given
		ClockUtil.CurrentTime = today;
		const long hourInMs = 60 * 60 * 1000;

		string key = "oneFailingServiceTaskProcess";

		// a running process instance with a failed service task
		IDictionary<string, object> @params = new Dictionary<string, object>();
		@params["fail"] = true;
		runtimeService.startProcessInstanceByKey(key, @params);

		// when
		// the process definition will be suspended
		repositoryService.suspendProcessDefinitionByKey(key, false, new DateTime(today.Ticks + (2 * hourInMs)));

		// then
		// there exists a timer job to suspend the process definition delayed
		Job timerToSuspendProcessDefinition = managementService.createJobQuery().timers().singleResult();
		assertNotNull(timerToSuspendProcessDefinition);

		// there is a user log entry for the activation
		long? processDefinitionEntryCount = query().entityType(PROCESS_DEFINITION).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION).processDefinitionKey(key).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).count();

		assertEquals(1, processDefinitionEntryCount.Value);

		// when
		// execute job
		managementService.executeJob(timerToSuspendProcessDefinition.Id);

		// then
		// there is a user log entry for the activation
		processDefinitionEntryCount = query().entityType(PROCESS_DEFINITION).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SUSPEND_PROCESS_DEFINITION).processDefinitionKey(key).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR).count();

		assertEquals(1, processDefinitionEntryCount.Value);

		// clean up op log
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<object>
	  {
		  private readonly UserOperationLogQueryTest outerInstance;

		  public CommandAnonymousInnerClass2(UserOperationLogQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  // ----- PROCESS INSTANCE MODIFICATION -----

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryProcessInstanceModificationOperation()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		string processInstanceId = processInstance.Id;

		ProcessDefinition definition = repositoryService.createProcessDefinitionQuery().singleResult();

		runtimeService.createProcessInstanceModification(processInstance.Id).startBeforeActivity("theTask").execute();

		UserOperationLogQuery logQuery = query().entityType(EntityTypes.PROCESS_INSTANCE).operationType(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE);

		assertEquals(1, logQuery.count());
		UserOperationLogEntry logEntry = logQuery.singleResult();

		assertEquals(processInstanceId, logEntry.ProcessInstanceId);
		assertEquals(processInstance.ProcessDefinitionId, logEntry.ProcessDefinitionId);
		assertEquals(definition.Key, logEntry.ProcessDefinitionKey);
		assertEquals(deploymentId, logEntry.DeploymentId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_PROCESS_INSTANCE, logEntry.OperationType);
		assertEquals(EntityTypes.PROCESS_INSTANCE, logEntry.EntityType);
		assertNull(logEntry.Property);
		assertNull(logEntry.OrgValue);
		assertNull(logEntry.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
	  }

	  // ----- ADD VARIABLES -----

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddExecutionVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.setVariable(process.Id, "testVariable1", "THIS IS TESTVARIABLE!!!");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddExecutionVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.setVariables(process.Id, createMapForVariableAddition());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddExecutionVariablesSingleAndMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.setVariable(process.Id, "testVariable3", "foo");
		runtimeService.setVariables(process.Id, createMapForVariableAddition());
		runtimeService.setVariable(process.Id, "testVariable4", "bar");

		// then
		verifyVariableOperationAsserts(3, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddTaskVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariable(processTaskId, "testVariable1", "THIS IS TESTVARIABLE!!!");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddTaskVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariables(processTaskId, createMapForVariableAddition());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryAddTaskVariablesSingleAndMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setVariable(processTaskId, "testVariable3", "foo");
		taskService.setVariables(processTaskId, createMapForVariableAddition());
		taskService.setVariable(processTaskId, "testVariable4", "bar");

		// then
		verifyVariableOperationAsserts(3, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_SET_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  // ----- PATCH VARIABLES -----

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryPatchExecutionVariablesOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		((RuntimeServiceImpl) runtimeService).updateVariables(process.Id, createMapForVariableAddition(), createCollectionForVariableDeletion());

		// then
	   verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryPatchTaskVariablesOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		((TaskServiceImpl) taskService).updateVariablesLocal(processTaskId, createMapForVariableAddition(), createCollectionForVariableDeletion());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_MODIFY_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  // ----- REMOVE VARIABLES -----

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveExecutionVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.removeVariable(process.Id, "testVariable1");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveExecutionVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.removeVariables(process.Id, createCollectionForVariableDeletion());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveExecutionVariablesSingleAndMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");

		// when
		runtimeService.removeVariable(process.Id, "testVariable1");
		runtimeService.removeVariables(process.Id, createCollectionForVariableDeletion());
		runtimeService.removeVariable(process.Id, "testVariable2");

		// then
		verifyVariableOperationAsserts(3, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveTaskVariableOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.removeVariable(processTaskId, "testVariable1");

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveTaskVariablesMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.removeVariables(processTaskId, createCollectionForVariableDeletion());

		// then
		verifyVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryRemoveTaskVariablesSingleAndMapOperation()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.removeVariable(processTaskId, "testVariable3");
		taskService.removeVariables(processTaskId, createCollectionForVariableDeletion());
		taskService.removeVariable(processTaskId, "testVariable4");

		// then
		verifyVariableOperationAsserts(3, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_REMOVE_VARIABLE, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryByEntityTypes()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setAssignee(processTaskId, "foo");
		taskService.setVariable(processTaskId, "foo", "bar");

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().entityTypeIn(EntityTypes.TASK, EntityTypes.VARIABLE);

		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryByInvalidEntityTypes()
	  {
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().entityTypeIn("foo");

		verifyQueryResults(query, 0);

		try
		{
		  query.entityTypeIn((string[]) null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  query.entityTypeIn(EntityTypes.TASK, null, EntityTypes.VARIABLE);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryByCategories()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// when
		taskService.setAssignee(processTaskId, "foo");
		taskService.setVariable(processTaskId, "foo", "bar");

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		verifyQueryResults(query, 3);

		// and
		query = historyService.createUserOperationLogQuery().categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);

		verifyQueryResults(query, 2);

		// and
		query = historyService.createUserOperationLogQuery().category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByInvalidCategories()
	  {
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().categoryIn("foo");

		verifyQueryResults(query, 0);

		query = historyService.createUserOperationLogQuery().category("foo");

		verifyQueryResults(query, 0);

		try
		{
		  query.category(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  query.categoryIn((string[]) null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}

		try
		{
		  query.categoryIn(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN, null, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  // ----- DELETE VARIABLE HISTORY -----

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
		verifySingleVariableOperationPropertyChange("name", "testVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnHistoricInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
		verifySingleVariableOperationPropertyChange("name", "testVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnTaskOfRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariable(processTaskId, "testVariable", "test");
		taskService.setVariable(processTaskId, "testVariable", "test2");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
		verifySingleVariableOperationPropertyChange("name", "testVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnTaskOfHistoricInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		processTaskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariable(processTaskId, "testVariable", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
		verifySingleVariableOperationPropertyChange("name", "testVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnCase()
	  {
		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");
		caseService.setVariable(caseInstance.Id, "myVariable", 1);
		caseService.setVariable(caseInstance.Id, "myVariable", 2);
		caseService.setVariable(caseInstance.Id, "myVariable", 3);
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		verfiySingleCaseVariableOperationAsserts(caseInstance);
		verifySingleVariableOperationPropertyChange("name", "myVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/cmmn/oneTaskCase.cmmn"})]
	  public virtual void testQueryDeleteVariableHistoryOperationOnTaskOfCase()
	  {
		// given
		CaseInstance caseInstance = caseService.createCaseInstanceByKey("oneTaskCase");
		processTaskId = taskService.createTaskQuery().singleResult().Id;
		taskService.setVariable(processTaskId, "myVariable", "1");
		taskService.setVariable(processTaskId, "myVariable", "2");
		taskService.setVariable(processTaskId, "myVariable", "3");
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		verfiySingleCaseVariableOperationAsserts(caseInstance);
		verifySingleVariableOperationPropertyChange("name", "myVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  public virtual void testQueryDeleteVariableHistoryOperationOnStandaloneTask()
	  {
		// given
		Task task = taskService.newTask();
		taskService.saveTask(task);
		taskService.setVariable(task.Id, "testVariable", "testValue");
		taskService.setVariable(task.Id, "testVariable", "testValue2");
		HistoricVariableInstance variableInstance = historyService.createHistoricVariableInstanceQuery().singleResult();

		// when
		historyService.deleteHistoricVariableInstance(variableInstance.Id);

		// then
		string operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY;
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(1, logQuery.count());

		UserOperationLogEntry logEntry = logQuery.singleResult();
		assertEquals(task.Id, logEntry.TaskId);
		assertEquals(deploymentId, logEntry.DeploymentId);
		verifySingleVariableOperationPropertyChange("name", "testVariable", org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);

		taskService.deleteTask(task.Id, true);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariablesHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test2");
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariablesHistoryOperationOnHistoryInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		assertEquals(2, historyService.createHistoricVariableInstanceQuery().count());

		// when
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyHistoricVariableOperationAsserts(1, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableAndVariablesHistoryOperationOnRunningInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable", "test2");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test2");
		runtimeService.setVariable(process.Id, "testVariable3", "test");
		runtimeService.setVariable(process.Id, "testVariable3", "test2");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().variableName("testVariable").singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyHistoricVariableOperationAsserts(2, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  [Deployment(resources : {ONE_TASK_PROCESS})]
	  public virtual void testQueryDeleteVariableAndVariablesHistoryOperationOnHistoryInstance()
	  {
		// given
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		runtimeService.setVariable(process.Id, "testVariable", "test");
		runtimeService.setVariable(process.Id, "testVariable2", "test");
		runtimeService.setVariable(process.Id, "testVariable3", "test");
		runtimeService.deleteProcessInstance(process.Id, "none");
		string variableInstanceId = historyService.createHistoricVariableInstanceQuery().variableName("testVariable").singleResult().Id;

		// when
		historyService.deleteHistoricVariableInstance(variableInstanceId);
		historyService.deleteHistoricVariableInstancesByProcessInstanceId(process.Id);

		// then
		verifyHistoricVariableOperationAsserts(2, org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY);
	  }

	  // --------------- CMMN --------------------

	  [Deployment(resources:{ONE_TASK_CASE})]
	  public virtual void testQueryByCaseDefinitionId()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		taskService.setAssignee(task.Id, "demo");

		// then

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseDefinitionId(caseDefinitionId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{ONE_TASK_CASE})]
	  public virtual void testQueryByCaseInstanceId()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		string caseInstanceId = caseService.withCaseDefinition(caseDefinitionId).create().Id;

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		taskService.setAssignee(task.Id, "demo");

		// then

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseInstanceId(caseInstanceId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{ONE_TASK_CASE})]
	  public virtual void testQueryByCaseExecutionId()
	  {
		// given:
		// a deployed case definition
		string caseDefinitionId = repositoryService.createCaseDefinitionQuery().singleResult().Id;

		// an active case instance
		caseService.withCaseDefinition(caseDefinitionId).create();

		string caseExecutionId = caseService.createCaseExecutionQuery().activityId("PI_HumanTask_1").singleResult().Id;

		Task task = taskService.createTaskQuery().singleResult();

		assertNotNull(task);

		// when
		taskService.setAssignee(task.Id, "demo");

		// then

		UserOperationLogQuery query = historyService.createUserOperationLogQuery().caseExecutionId(caseExecutionId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_TASK_WORKER);

		verifyQueryResults(query, 1);
	  }

	  [Deployment(resources:{ONE_EXTERNAL_TASK_PROCESS})]
	  public virtual void testQueryByExternalTaskId()
	  {
		// given:
		// an active process instance
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		ExternalTask task = externalTaskService.createExternalTaskQuery().singleResult();
		assertNotNull(task);

		// when
		externalTaskService.setRetries(task.Id, 5);

		// then
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().externalTaskId(task.Id);

		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryByDeploymentId()
	  {
		// given
		string deploymentId = repositoryService.createDeployment().addClasspathResource(ONE_TASK_PROCESS).deploy().Id;

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().deploymentId(deploymentId).category(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR);

		// then
		verifyQueryResults(query, 1);

		repositoryService.deleteDeployment(deploymentId, true);
	  }

	  public virtual void testQueryByInvalidDeploymentId()
	  {
		UserOperationLogQuery query = historyService.createUserOperationLogQuery().deploymentId("invalid");

		verifyQueryResults(query, 0);

		try
		{
		  query.deploymentId(null);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // expected
		}
	  }

	  private void verifyQueryResults(UserOperationLogQuery query, int countExpected)
	  {
		assertEquals(countExpected, query.list().size());
		assertEquals(countExpected, query.count());

		if (countExpected == 1)
		{
		  assertNotNull(query.singleResult());
		}
		else if (countExpected > 1)
		{
		  verifySingleResultFails(query);
		}
		else if (countExpected == 0)
		{
		  assertNull(query.singleResult());
		}
	  }

	  private void verifySingleResultFails(UserOperationLogQuery query)
	  {
		try
		{
		  query.singleResult();
		  fail();
		}
		catch (ProcessEngineException)
		{
		}
	  }

	  private IDictionary<string, object> createMapForVariableAddition()
	  {
		IDictionary<string, object> variables = new Dictionary<string, object>();
		variables["testVariable1"] = "THIS IS TESTVARIABLE!!!";
		variables["testVariable2"] = "OVER 9000!";

		return variables;
	  }

	  private ICollection<string> createCollectionForVariableDeletion()
	  {
		ICollection<string> variables = new List<string>();
		variables.Add("testVariable3");
		variables.Add("testVariable4");

		return variables;
	  }

	  private void verifyVariableOperationAsserts(int countAssertValue, string operationType, string category)
	  {
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(countAssertValue, logQuery.count());

		if (countAssertValue > 1)
		{
		  IList<UserOperationLogEntry> logEntryList = logQuery.list();

		  foreach (UserOperationLogEntry logEntry in logEntryList)
		  {
			assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
			assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
			assertEquals(deploymentId, logEntry.DeploymentId);
			assertEquals(category, logEntry.Category);
		  }
		}
		else
		{
		  UserOperationLogEntry logEntry = logQuery.singleResult();
		  assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
		  assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
		  assertEquals(deploymentId, logEntry.DeploymentId);
		  assertEquals(category, logEntry.Category);
		}
	  }

	  private void verifyHistoricVariableOperationAsserts(int countAssertValue, string operationType)
	  {
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(countAssertValue, logQuery.count());

		if (countAssertValue > 1)
		{
		  IList<UserOperationLogEntry> logEntryList = logQuery.list();

		  foreach (UserOperationLogEntry logEntry in logEntryList)
		  {
			assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
			assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
			assertEquals(deploymentId, logEntry.DeploymentId);
			assertNull(logEntry.TaskId);
			assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
		  }
		}
		else
		{
		  UserOperationLogEntry logEntry = logQuery.singleResult();
		  assertEquals(process.ProcessDefinitionId, logEntry.ProcessDefinitionId);
		  assertEquals(process.ProcessInstanceId, logEntry.ProcessInstanceId);
		  assertEquals(deploymentId, logEntry.DeploymentId);
		  assertNull(logEntry.TaskId);
		  assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
		}
	  }

	  private void verifySingleVariableOperationPropertyChange(string property, string newValue, string operationType)
	  {
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(1, logQuery.count());
		UserOperationLogEntry logEntry = logQuery.singleResult();
		assertEquals(property, logEntry.Property);
		assertEquals(newValue, logEntry.NewValue);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
	  }

	  private void verfiySingleCaseVariableOperationAsserts(CaseInstance caseInstance)
	  {
		string operationType = org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE_HISTORY;
		UserOperationLogQuery logQuery = query().entityType(EntityTypes.VARIABLE).operationType(operationType);
		assertEquals(1, logQuery.count());

		UserOperationLogEntry logEntry = logQuery.singleResult();
		assertEquals(caseInstance.CaseDefinitionId, logEntry.CaseDefinitionId);
		assertEquals(caseInstance.CaseInstanceId, logEntry.CaseInstanceId);
		assertEquals(deploymentId, logEntry.DeploymentId);
		assertNull(logEntry.TaskId);
		assertEquals(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_OPERATOR, logEntry.Category);
	  }

	  private UserOperationLogQuery query()
	  {
		return historyService.createUserOperationLogQuery();
	  }

	  /// <summary>
	  /// start process and operate on userTask to create some log entries for the query tests
	  /// </summary>
	  private void createLogEntries()
	  {
		ClockUtil.CurrentTime = yesterday;

		// create a process with a userTask and work with it
		process = runtimeService.startProcessInstanceByKey("oneTaskProcess");
		execution = processEngine.RuntimeService.createExecutionQuery().processInstanceId(process.Id).singleResult();
		processTaskId = taskService.createTaskQuery().singleResult().Id;

		// user "icke" works on the process userTask
		identityService.AuthenticatedUserId = "icke";

		// create and remove some links
		taskService.addCandidateUser(processTaskId, "er");
		taskService.deleteCandidateUser(processTaskId, "er");
		taskService.addCandidateGroup(processTaskId, "wir");
		taskService.deleteCandidateGroup(processTaskId, "wir");

		// assign and reassign the userTask
		ClockUtil.CurrentTime = today;
		taskService.setOwner(processTaskId, "icke");
		taskService.claim(processTaskId, "icke");
		taskService.setAssignee(processTaskId, "er");

		// change priority of task
		taskService.setPriority(processTaskId, 10);

		// add and delete an attachment
		Attachment attachment = taskService.createAttachment("image/ico", processTaskId, process.Id, "favicon.ico", "favicon", "http://camunda.com/favicon.ico");
		taskService.deleteAttachment(attachment.Id);

		// complete the userTask to finish the process
		taskService.complete(processTaskId);
		assertProcessEnded(process.Id);

		// user "er" works on the process userTask
		identityService.AuthenticatedUserId = "er";

		// create a standalone userTask
		userTask = taskService.newTask();
		userTask.Name = "to do";
		taskService.saveTask(userTask);

		// change some properties manually to create an update event
		ClockUtil.CurrentTime = tomorrow;
		userTask.Description = "desc";
		userTask.Owner = "icke";
		userTask.Assignee = "er";
		userTask.DueDate = DateTime.Now;
		taskService.saveTask(userTask);

		// complete the userTask
		taskService.complete(userTask.Id);
	  }

	}

}