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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;


	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using HistoryLevel = org.camunda.bpm.engine.impl.history.HistoryLevel;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerSuspendProcessDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendProcessDefinitionHandler;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using IncidentQuery = org.camunda.bpm.engine.runtime.IncidentQuery;
	using Job = org.camunda.bpm.engine.runtime.Job;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class IncidentAuthorizationTest : AuthorizationTest
	{

	  protected internal const string TIMER_START_PROCESS_KEY = "timerStartProcess";
	  protected internal const string ONE_INCIDENT_PROCESS_KEY = "process";
	  protected internal const string ANOTHER_ONE_INCIDENT_PROCESS_KEY = "anotherOneIncidentProcess";

	  protected internal new string deploymentId;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		deploymentId = createDeployment(null, "org/camunda/bpm/engine/test/api/authorization/timerStartEventProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/oneIncidentProcess.bpmn20.xml", "org/camunda/bpm/engine/test/api/authorization/anotherOneIncidentProcess.bpmn20.xml").Id;
		base.setUp();
	  }

	  public override void tearDown()
	  {
		base.tearDown();
		deleteDeployment(deploymentId);
	  }

	  public virtual void testQueryForStandaloneIncidents()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string jobId = null;
		IList<Job> jobs = managementService.createJobQuery().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			jobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testStartTimerJobIncidentQueryWithoutAuthorization()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testStartTimerJobIncidentQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testStartTimerJobIncidentQueryWithReadInstancePermissionOnProcessDefinition()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testStartTimerJobIncidentQueryWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testSimpleQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		Incident incident = query.singleResult();
		assertNotNull(incident);
		assertEquals(processInstanceId, incident.ProcessInstanceId);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnOneTaskProcess()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 3);
	  }

	  public virtual void testQueryWithReadInstancesPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		IncidentQuery query = runtimeService.createIncidentQuery();

		// then
		verifyQueryResults(query, 7);
	  }

	  protected internal virtual void verifyQueryResults(IncidentQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual void clearDatabase()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private readonly IncidentAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(IncidentAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			HistoryLevel historyLevel = Context.ProcessEngineConfiguration.HistoryLevel;
			if (historyLevel.Equals(org.camunda.bpm.engine.impl.history.HistoryLevel_Fields.HISTORY_LEVEL_FULL))
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendProcessDefinitionHandler.TYPE);
			  IList<HistoricIncident> incidents = Context.ProcessEngineConfiguration.HistoryService.createHistoricIncidentQuery().list();
			  foreach (HistoricIncident incident in incidents)
			  {
				commandContext.HistoricIncidentManager.delete((HistoricIncidentEntity) incident);
			  }
			}

			return null;
		  }
	  }

	}

}