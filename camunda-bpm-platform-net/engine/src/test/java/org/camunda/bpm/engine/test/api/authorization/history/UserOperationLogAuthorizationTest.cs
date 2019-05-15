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
namespace org.camunda.bpm.engine.test.api.authorization.history
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.DELETE_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;


	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using UserOperationLogQuery = org.camunda.bpm.engine.history.UserOperationLogQuery;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class UserOperationLogAuthorizationTest : AuthorizationTest
	{

	  protected internal const string ONE_TASK_PROCESS_KEY = "oneTaskProcess";
	  protected internal const string ONE_TASK_CASE_KEY = "oneTaskCase";
	  protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/oneTaskProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneTaskCase.cmmn", "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  // standalone task ///////////////////////////////

	  public virtual void testQueryCreateStandaloneTaskUserOperationLog()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		deleteTask(taskId, true);
	  }

	  public virtual void testQuerySetAssigneeStandaloneTaskUserOperationLog()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		deleteTask(taskId, true);
	  }

	  // (process) user task /////////////////////////////

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetAssigneeTaskUserOperationLogWithMultiple()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // (case) human task /////////////////////////////

	  public virtual void testQuerySetAssigneeHumanTaskUserOperationLog()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // standalone job ///////////////////////////////

	  public virtual void testQuerySetStandaloneJobRetriesUserOperationLog()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  // job ///////////////////////////////

	  public virtual void testQuerySetJobRetriesUserOperationLogWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQuerySetJobRetriesUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobId = selectSingleJob().Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 5);
		enableAuthorization();

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // process definition ////////////////////////////////////////////

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithoutAuthorization()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessDefinitionUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		suspendProcessDefinitionByKey(ONE_TASK_PROCESS_KEY);
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 1);

		clearDatabase();
	  }

	  // process instance //////////////////////////////////////////////

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 0);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  public virtual void testQuerySuspendProcessInstanceUserOperationLogWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		suspendProcessInstanceById(processInstanceId);

		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 2);

		clearDatabase();
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorizationWithoutAuthentication(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		// when
		UserOperationLogQuery query = historyService.createUserOperationLogQuery();

		// then
		verifyQueryResults(query, 3);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // delete user operation log (standalone) ////////////////////////

	  public virtual void testDeleteStandaloneEntry()
	  {
		// given
		string taskId = "myTask";
		createTask(taskId);

		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());

		deleteTask(taskId, true);
	  }

	  // delete user operation log /////////////////////////////////////

	  public virtual void testDeleteEntryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		try
		{
		  // when
		  historyService.deleteUserOperationLogEntry(entryId);
		  fail("Exception expected: It should not be possible to delete the user operation log");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(DELETE_HISTORY.Name, message);
		  assertTextPresent(ONE_TASK_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteEntryWithDeleteHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, DELETE_HISTORY);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryWithDeleteHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(ONE_TASK_PROCESS_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, DELETE_HISTORY);

		disableAuthorization();
		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;
		enableAuthorization();

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();
	  }

	  public virtual void testDeleteEntryAfterDeletingDeployment()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(ONE_TASK_PROCESS_KEY).Id;
		string taskId = selectSingleTask().Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_TASK_PROCESS_KEY, userId, READ_HISTORY, DELETE_HISTORY);

		disableAuthorization();
		taskService.complete(taskId);
		enableAuthorization();

		deleteDeployment(deploymentId, false);

		string entryId = historyService.createUserOperationLogQuery().entityType("Task").singleResult().Id;

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		disableAuthorization();
		assertNull(historyService.createUserOperationLogQuery().entityType("Task").singleResult());
		enableAuthorization();

		disableAuthorization();
		historyService.deleteHistoricProcessInstance(processInstanceId);
		enableAuthorization();
	  }

	  // delete user operation log (case) //////////////////////////////

	  public virtual void testCaseDeleteEntry()
	  {
		// given
		createCaseInstanceByKey(ONE_TASK_CASE_KEY);
		string taskId = selectSingleTask().Id;
		setAssignee(taskId, "demo");

		string entryId = historyService.createUserOperationLogQuery().singleResult().Id;

		// when
		historyService.deleteUserOperationLogEntry(entryId);

		// then
		assertNull(historyService.createUserOperationLogQuery().singleResult());
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(UserOperationLogQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual Job selectSingleJob()
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().singleResult();
		enableAuthorization();
		return job;
	  }

	  protected internal virtual void clearDatabase()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly UserOperationLogAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(UserOperationLogAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			IList<HistoricIncident> incidents = Context.ProcessEngineConfiguration.HistoryService.createHistoricIncidentQuery().list();
			foreach (HistoricIncident incident in incidents)
			{
			  commandContext.HistoricIncidentManager.delete((HistoricIncidentEntity) incident);
			}
			return null;
		  }
	  }
	}

}