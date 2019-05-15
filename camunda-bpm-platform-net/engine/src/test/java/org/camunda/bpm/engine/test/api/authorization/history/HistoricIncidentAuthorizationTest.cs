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


	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricIncidentQuery = org.camunda.bpm.engine.history.HistoricIncidentQuery;
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
	public class HistoricIncidentAuthorizationTest : AuthorizationTest
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

	  // historic incident query (standalone) //////////////////////////////

	  public virtual void testQueryForStandaloneHistoricIncidents()
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
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);

		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();

		clearDatabase();
	  }

	  // historic incident query (start timer job incident) //////////////////////////////

	  public virtual void testStartTimerJobIncidentQueryWithoutAuthorization()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testStartTimerJobIncidentQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testStartTimerJobIncidentQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		disableAuthorization();
		string jobId = managementService.createJobQuery().singleResult().Id;
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

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

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic incident query ///////////////////////////////////////////

	  public virtual void testSimpleQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testSimpleQueryWithMultiple()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  // historic incident query (multiple incidents ) ///////////////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 5);
	  }

	  // historic job log (mixed) //////////////////////////////////////////

	  public virtual void testMixedQueryWithoutAuthorization()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string firstJobId = null;
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			firstJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(firstJobId, 0);

		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string secondJobId = null;
		jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			secondJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(secondJobId, 0);
		enableAuthorization();

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 2);

		disableAuthorization();
		managementService.deleteJob(firstJobId);
		managementService.deleteJob(secondJobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testMixedQueryWithReadHistoryPermissionOnProcessDefinition()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string firstJobId = null;
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			firstJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(firstJobId, 0);

		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string secondJobId = null;
		jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			secondJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(secondJobId, 0);
		enableAuthorization();

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 4);

		disableAuthorization();
		managementService.deleteJob(firstJobId);
		managementService.deleteJob(secondJobId);
		enableAuthorization();

		clearDatabase();
	  }

	  public virtual void testMixedQueryWithReadHistoryPermissionOnAnyProcessDefinition()
	  {
		// given
		disableAuthorization();
		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string firstJobId = null;
		IList<Job> jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			firstJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(firstJobId, 0);

		repositoryService.suspendProcessDefinitionByKey(ONE_INCIDENT_PROCESS_KEY, true, DateTime.Now);
		string secondJobId = null;
		jobs = managementService.createJobQuery().withRetriesLeft().list();
		foreach (Job job in jobs)
		{
		  if (string.ReferenceEquals(job.ProcessDefinitionKey, null))
		  {
			secondJobId = job.Id;
			break;
		  }
		}
		managementService.setJobRetries(secondJobId, 0);
		enableAuthorization();

		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY);

		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);
		startProcessAndExecuteJob(ANOTHER_ONE_INCIDENT_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_HISTORY);

		// when
		HistoricIncidentQuery query = historyService.createHistoricIncidentQuery();

		// then
		verifyQueryResults(query, 7);

		disableAuthorization();
		managementService.deleteJob(firstJobId);
		managementService.deleteJob(secondJobId);
		enableAuthorization();

		clearDatabase();
	  }

	  // helper ////////////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(HistoricIncidentQuery query, int countExpected)
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
		  private readonly HistoricIncidentAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(HistoricIncidentAuthorizationTest outerInstance)
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