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
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_HISTORY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;


	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using HistoricJobLogQuery = org.camunda.bpm.engine.history.HistoricJobLogQuery;
	using HistoricProcessInstance = org.camunda.bpm.engine.history.HistoricProcessInstance;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class HistoricJobLogAuthorizationTest : AuthorizationTest
	{

	  protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
	  protected internal const string TIMER_BOUNDARY_PROCESS_KEY = "timerBoundaryProcess";
	  protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/timerStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/timerBoundaryEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
		deleteDeployment(deploymentId);
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly HistoricJobLogAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(HistoricJobLogAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  // historic job log query (start timer job) ////////////////////////////////

	  public virtual void testStartTimerJobLogQueryWithoutAuthorization()
	  {
		// given

		// when

		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testStartTimerJobLogQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testStartTimerJobLogQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic job log query ////////////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithHistoryReadPermissionOnProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 4);
	  }

	  public virtual void testSimpleQueryWithHistoryReadPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 5);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 5);
	  }

	  // historic job log query (multiple process instance) ////////////////////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		enableAuthorization();

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithHistoryReadPermissionOnProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 12);
	  }

	  public virtual void testQueryWithHistoryReadPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		jobId = managementService.createJobQuery().processDefinitionKey(TIMER_START_PROCESS_KEY).singleResult().Id;
		managementService.executeJob(jobId);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 17);
	  }

	  // historic job log query (standalone job) ///////////////////////

	  public virtual void testQueryAfterStandaloneJob()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 1);

		HistoricJobLog jobLog = query.singleResult();
		assertNull(jobLog.ProcessDefinitionKey);

		deleteDeployment(deploymentId);

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.deleteJob(jobId);
		enableAuthorization();
	  }

	  // delete deployment (cascade = false)

	  public virtual void testQueryAfterDeletingDeployment()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, READ_HISTORY);

		disableAuthorization();
		IList<Task> tasks = taskService.createTaskQuery().list();
		foreach (Task task in tasks)
		{
		  taskService.complete(task.Id);
		}
		enableAuthorization();

		disableAuthorization();
		repositoryService.deleteDeployment(deploymentId);
		enableAuthorization();

		// when
		HistoricJobLogQuery query = historyService.createHistoricJobLogQuery();

		// then
		verifyQueryResults(query, 6);

		disableAuthorization();
		IList<HistoricProcessInstance> instances = historyService.createHistoricProcessInstanceQuery().list();
		foreach (HistoricProcessInstance instance in instances)
		{
		  historyService.deleteHistoricProcessInstance(instance.Id);
		}
		enableAuthorization();
	  }

	  // get historic job log exception stacktrace (standalone) /////////////////////

	  public virtual void testGetHistoricStandaloneJobLogExceptionStacktrace()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY, true, DateTime.Now);
		enableAuthorization();
		string jobLogId = historyService.createHistoricJobLogQuery().singleResult().Id;

		// when
		string stacktrace = historyService.getHistoricJobLogExceptionStacktrace(jobLogId);

		// then
		assertNull(stacktrace);

		deleteDeployment(deploymentId);

		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.deleteJob(jobId);
		enableAuthorization();
	  }

	  // get historic job log exception stacktrace /////////////////////

	  public virtual void testGetHistoricJobLogExceptionStacktraceWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobLogId = historyService.createHistoricJobLogQuery().failureLog().listPage(0, 1).get(0).Id;
		enableAuthorization();

		try
		{
		  // when
		  historyService.getHistoricJobLogExceptionStacktrace(jobLogId);
		  fail("Exception expected: It should not be possible to get the historic job log exception stacktrace");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ_HISTORY.Name, message);
		  assertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetHistoricJobLogExceptionStacktraceWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobLogId = historyService.createHistoricJobLogQuery().failureLog().listPage(0, 1).get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		string stacktrace = historyService.getHistoricJobLogExceptionStacktrace(jobLogId);

		// then
		assertNotNull(stacktrace);
	  }

	  public virtual void testGetHistoricJobLogExceptionStacktraceWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		disableAuthorization();
		string jobLogId = historyService.createHistoricJobLogQuery().failureLog().listPage(0, 1).get(0).Id;
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		string stacktrace = historyService.getHistoricJobLogExceptionStacktrace(jobLogId);

		// then
		assertNotNull(stacktrace);
	  }

	  // helper ////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricJobLogQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	}

}