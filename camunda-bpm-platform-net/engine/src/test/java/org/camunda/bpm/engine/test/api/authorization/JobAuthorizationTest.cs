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
namespace org.camunda.bpm.engine.test.api.authorization
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Authorization_Fields.ANY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.READ_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Permissions.UPDATE_INSTANCE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_DEFINITION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.authorization.Resources.PROCESS_INSTANCE;

	using ProcessDefinitionPermissions = org.camunda.bpm.engine.authorization.ProcessDefinitionPermissions;
	using ProcessInstancePermissions = org.camunda.bpm.engine.authorization.ProcessInstancePermissions;
	using AbstractQuery = org.camunda.bpm.engine.impl.AbstractQuery;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using TimerSuspendJobDefinitionHandler = org.camunda.bpm.engine.impl.jobexecutor.TimerSuspendJobDefinitionHandler;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class JobAuthorizationTest : AuthorizationTest
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
		  private readonly JobAuthorizationTest outerInstance;

		  public CommandAnonymousInnerClass(JobAuthorizationTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public object execute(CommandContext commandContext)
		  {
			commandContext.HistoricJobLogManager.deleteHistoricJobLogsByHandlerType(TimerSuspendJobDefinitionHandler.TYPE);
			return null;
		  }
	  }

	  // job query (jobs associated to a process) //////////////////////////////////////////////////

	  public virtual void testQueryWithoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 0);
	  }

	  public virtual void testQueryWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 1);
	  }

	  public virtual void testQueryWithReadPermissionOnAnyProcessInstance()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithMultiple()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  public virtual void testQueryWithReadInstancePermissionOnTimerStartProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 1);

		Job job = query.singleResult();
		assertNull(job.ProcessInstanceId);
		assertEquals(TIMER_START_PROCESS_KEY, job.ProcessDefinitionKey);
	  }

	  public virtual void testQueryWithReadInstancePermissionOnTimerBoundaryProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 1);

		Job job = query.singleResult();
		assertEquals(processInstanceId, job.ProcessInstanceId);
		assertEquals(TIMER_BOUNDARY_PROCESS_KEY, job.ProcessDefinitionKey);
	  }

	  public virtual void testQueryWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 2);
	  }

	  // job query (standalone job) /////////////////////////////////

	  public virtual void testStandaloneJobQueryWithoutAuthorization()
	  {
		// given
		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, true, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		// when
		JobQuery query = managementService.createJobQuery();

		// then
		verifyQueryResults(query, 1);

		Job job = query.singleResult();
		assertNotNull(job);
		assertNull(job.ProcessInstanceId);
		assertNull(job.ProcessDefinitionKey);

		deleteJob(job.Id);
	  }

	  // execute job ////////////////////////////////////////////////

	  public virtual void testExecuteJobWithoutAuthorization()
	  {
		// given
		Job job = selectAnyJob();
		string jobId = job.Id;

		try
		{
		  // when
		  managementService.executeJob(jobId);
		  fail("Exception expected: It should not be possible to execute the job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(job.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testExecuteJobWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.executeJob(jobId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testExecuteJobWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.executeJob(jobId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testExecuteJobWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.executeJob(jobId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  public virtual void testExecuteJobWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.executeJob(jobId);

		// then
		Task task = selectSingleTask();
		assertNotNull(task);
		assertEquals("taskAfterBoundaryEvent", task.TaskDefinitionKey);
	  }

	  // execute job (standalone job) ////////////////////////////////

	  public virtual void testExecuteStandaloneJob()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.executeJob(jobId);

		// then
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY);
		assertTrue(jobDefinition.Suspended);
	  }

	  // delete job ////////////////////////////////////////////////

	  public virtual void testDeleteJobWithoutAuthorization()
	  {
		// given
		Job job = selectAnyJob();
		string jobId = job.Id;

		try
		{
		  // when
		  managementService.deleteJob(jobId);
		  fail("Exception expected: It should not be possible to delete the job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(job.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testDeleteJobWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.deleteJob(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNull(job);
	  }

	  public virtual void testDeleteJobWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.deleteJob(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNull(job);
	  }

	  public virtual void testDeleteJobWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.deleteJob(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNull(job);
	  }

	  public virtual void testDeleteJobWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		// when
		managementService.deleteJob(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNull(job);
	  }

	  // delete standalone job ////////////////////////////////

	  public virtual void testDeleteStandaloneJob()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.deleteJob(jobId);

		// then
		Job job = selectJobById(jobId);
		assertNull(job);
	  }

	  // set job retries ////////////////////////////////////////////////

	  public virtual void testSetJobRetriesWithoutAuthorization()
	  {
		// given
		Job job = selectAnyJob();
		string jobId = job.Id;

		try
		{
		  // when
		  managementService.setJobRetries(jobId, 1);
		  fail("Exception expected: It should not be possible to set job retries");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(job.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(ProcessInstancePermissions.RETRY_JOB.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.RETRY_JOB.Name, message);
		}
	  }

	  public virtual void testSetJobRetriesWithRevokeAuthorizationRevokeRetryJobPermission()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, processInstanceId, userId, UPDATE);
		createRevokeAuthorization(PROCESS_DEFINITION, processInstanceId, userId, ProcessDefinitionPermissions.RETRY_JOB);
		Job job = selectJobByProcessInstanceId(processInstanceId);

		try
		{
		  // when
		  managementService.setJobRetries(job.Id, 1);
		  fail("Exception expected: It should not be possible to set job retries");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(job.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(ProcessInstancePermissions.RETRY_JOB.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.RETRY_JOB.Name, message);
		}
	  }

	  public virtual void testSetJobRetriesWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithRetryJobPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, ProcessInstancePermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithRetryJobPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithRetryJobInstancePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, ProcessDefinitionPermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesWithUpdateRetryJobPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, ProcessDefinitionPermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  // set job retries (standalone) ////////////////////////////////

	  public virtual void testSetStandaloneJobRetries()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.setJobRetries(jobId, 1);

		// then
		Job job = selectJobById(jobId);
		assertEquals(1, job.Retries);

		deleteJob(jobId);
	  }

	  // set job retries by job definition id ///////////////////////

	  public virtual void testSetJobRetriesByJobDefinitionIdWithoutAuthorization()
	  {
		// given
		disableAuthorization();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().listPage(0, 1).get(0);
		enableAuthorization();

		string jobDefinitionId = jobDefinition.Id;

		try
		{
		  // when
		  managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);
		  fail("Exception expected: It should not be possible to set job retries");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(jobDefinition.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(ProcessInstancePermissions.RETRY_JOB.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.RETRY_JOB.Name, message);
		}
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithRevokeRetryJobPermission()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, processInstanceId, userId, UPDATE);
		createRevokeAuthorization(PROCESS_DEFINITION, processInstanceId, userId, ProcessDefinitionPermissions.RETRY_JOB);
		JobDefinition jobDefinition = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		try
		{
		  // when
		  managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 1);
		  fail("Exception expected: It should not be possible to set job retries");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(jobDefinition.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(ProcessInstancePermissions.RETRY_JOB.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.RETRY_JOB.Name, message);
		}
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);
		  fail("Exception expected: It should not be possible to set job retries");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		  assertTextPresent(ProcessInstancePermissions.RETRY_JOB.Name, message);
		  assertTextPresent(ProcessDefinitionPermissions.RETRY_JOB.Name, message);
		}
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithRetryJobPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, ProcessInstancePermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithUpdateInstancePermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance processInstance = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, processInstance.ProcessDefinitionId, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithRetryJobPermissionOnProcessDefinition()
	  {
		// given
		ProcessInstance processInstance = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, processInstance.ProcessDefinitionId, userId, ProcessDefinitionPermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstance.Id).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdWithRetryJobPermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, ProcessDefinitionPermissions.RETRY_JOB);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		disableAuthorization();
		managementService.setJobRetries(jobId, 0);
		enableAuthorization();

		// when
		managementService.setJobRetriesByJobDefinitionId(jobDefinitionId, 1);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertEquals(1, job.Retries);
	  }

	  // set job due date ///////////////////////////////////////////

	  public virtual void testSetJobDueDateWithoutAuthorization()
	  {
		// given
		Job job = selectAnyJob();
		string jobId = job.Id;

		try
		{
		  // when
		  managementService.setJobDuedate(jobId, DateTime.Now);
		  fail("Exception expected: It should not be possible to set the job due date");
		}
		catch (AuthorizationException e)
		{
		  // then
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(job.ProcessDefinitionKey, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSetJobDueDateWithUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobDuedate(jobId, null);

		// then
		Job job = selectJobById(jobId);
		assertNull(job.Duedate);
	  }

	  public virtual void testSetJobDueDateWithUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobDuedate(jobId, null);

		// then
		Job job = selectJobById(jobId);
		assertNull(job.Duedate);
	  }

	  public virtual void testSetJobDueDateWithUpdateInstancePermissionOnTimerBoundaryProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobDuedate(jobId, null);

		// then
		Job job = selectJobById(jobId);
		assertNull(job.Duedate);
	  }

	  public virtual void testSetJobDueDateWithUpdateInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		// when
		managementService.setJobDuedate(jobId, null);

		// then
		Job job = selectJobById(jobId);
		assertNull(job.Duedate);
	  }

	  // set job retries (standalone) ////////////////////////////////

	  public virtual void testSetStandaloneJobDueDate()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.setJobDuedate(jobId, null);

		// then
		Job job = selectJobById(jobId);
		assertNull(job.Duedate);

		deleteJob(jobId);
	  }

	  // get exception stacktrace ///////////////////////////////////////////

	  public virtual void testGetExceptionStacktraceWithoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		disableAuthorization();
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		try
		{
		  // when
		  managementService.getJobExceptionStacktrace(jobId);
		  fail("Exception expected: It should not be possible to get the exception stacktrace");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(READ.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(READ_INSTANCE.Name, message);
		  assertTextPresent(ONE_INCIDENT_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testGetExceptionStacktraceWithReadPermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, READ);

		// when
		string jobExceptionStacktrace = managementService.getJobExceptionStacktrace(jobId);

		// then
		assertNotNull(jobExceptionStacktrace);
	  }

	  public virtual void testGetExceptionStacktraceReadPermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, READ);

		// when
		string jobExceptionStacktrace = managementService.getJobExceptionStacktrace(jobId);

		// then
		assertNotNull(jobExceptionStacktrace);
	  }

	  public virtual void testGetExceptionStacktraceWithReadInstancePermissionOnTimerBoundaryProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ONE_INCIDENT_PROCESS_KEY, userId, READ_INSTANCE);

		// when
		string jobExceptionStacktrace = managementService.getJobExceptionStacktrace(jobId);

		// then
		assertNotNull(jobExceptionStacktrace);
	  }

	  public virtual void testGetExceptionStacktraceWithReadInstancePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessAndExecuteJob(ONE_INCIDENT_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, READ_INSTANCE);

		// when
		string jobExceptionStacktrace = managementService.getJobExceptionStacktrace(jobId);

		// then
		assertNotNull(jobExceptionStacktrace);
	  }

	  // get exception stacktrace (standalone) ////////////////////////////////

	  public virtual void testStandaloneJobGetExceptionStacktrace()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		string jobExceptionStacktrace = managementService.getJobExceptionStacktrace(jobId);

		// then
		assertNull(jobExceptionStacktrace);

		deleteJob(jobId);
	  }

	  // suspend job by id //////////////////////////////////////////

	  public virtual void testSuspendJobByIdWihtoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		try
		{
		  // when
		  managementService.suspendJobById(jobId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		managementService.suspendJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendStandaloneJobById()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;

		// when
		managementService.suspendJobById(jobId);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertTrue(job.Suspended);

		deleteJob(jobId);
	  }

	  // activate job by id //////////////////////////////////////////

	  public virtual void testActivateJobByIdWihtoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		suspendJobById(jobId);

		try
		{
		  // when
		  managementService.activateJobById(jobId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		suspendJobById(jobId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		managementService.activateJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		suspendJobById(jobId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		suspendJobById(jobId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobId = selectJobByProcessInstanceId(processInstanceId).Id;
		suspendJobById(jobId);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobById(jobId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateStandaloneJobById()
	  {
		// given
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_START_PROCESS_KEY, userId, UPDATE);

		DateTime startTime = DateTime.Now;
		ClockUtil.CurrentTime = startTime;
		long oneWeekFromStartTime = startTime.Ticks + (7 * 24 * 60 * 60 * 1000);

		disableAuthorization();
		// creates a new "standalone" job
		managementService.suspendJobDefinitionByProcessDefinitionKey(TIMER_START_PROCESS_KEY, false, new DateTime(oneWeekFromStartTime));
		enableAuthorization();

		string jobId = managementService.createJobQuery().singleResult().Id;
		suspendJobById(jobId);

		// when
		managementService.activateJobById(jobId);

		// then
		Job job = selectJobById(jobId);
		assertNotNull(job);
		assertFalse(job.Suspended);

		deleteJob(jobId);
	  }

	  // suspend job by process instance id //////////////////////////////////////////

	  public virtual void testSuspendJobByProcessInstanceIdWihtoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.suspendJobByProcessInstanceId(processInstanceId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		managementService.suspendJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job by process instance id //////////////////////////////////////////

	  public virtual void testActivateJobByProcessInstanceIdWihtoutAuthorization()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessInstanceId(processInstanceId);

		try
		{
		  // when
		  managementService.activateJobByProcessInstanceId(processInstanceId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(processInstanceId, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessInstanceId(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		// when
		managementService.activateJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessInstanceId(processInstanceId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessInstanceId(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessInstanceIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessInstanceId(processInstanceId);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessInstanceId(processInstanceId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // suspend job by job definition id //////////////////////////////////////////

	  public virtual void testSuspendJobByJobDefinitionIdWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.suspendJobByJobDefinitionId(jobDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobByJobDefinitionId(jobDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job by job definition id //////////////////////////////////////////

	  public virtual void testActivateJobByJobDefinitionIdWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByJobDefinitionId(jobDefinitionId);

		try
		{
		  // when
		  managementService.activateJobByJobDefinitionId(jobDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByJobDefinitionId(jobDefinitionId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobByJobDefinitionId(jobDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByJobDefinitionId(jobDefinitionId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByJobDefinitionId(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByJobDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string jobDefinitionId = selectJobDefinitionByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByJobDefinitionId(jobDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByJobDefinitionId(jobDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // suspend job by process definition id //////////////////////////////////////////

	  public virtual void testSuspendJobByProcessDefinitionIdWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		try
		{
		  // when
		  managementService.suspendJobByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job by process definition id //////////////////////////////////////////

	  public virtual void testActivateJobByProcessDefinitionIdWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionId(processDefinitionId);

		try
		{
		  // when
		  managementService.activateJobByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobByProcessDefinitionId(processDefinitionId);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessDefinitionIdWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		string processDefinitionId = selectProcessDefinitionByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionId(processDefinitionId);

		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessDefinitionId(processDefinitionId);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // suspend job by process definition key //////////////////////////////////////////

	  public virtual void testSuspendJobByProcessDefinitionKeyWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);

		try
		{
		  // when
		  managementService.suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to suspend a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  public virtual void testSuspendJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertTrue(job.Suspended);
	  }

	  // activate job by process definition key //////////////////////////////////////////

	  public virtual void testActivateJobByProcessDefinitionKeyWihtoutAuthorization()
	  {
		// given
		startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY);
		suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		try
		{
		  // when
		  managementService.activateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, processInstanceId, userId, UPDATE);

		try
		{
		  // when
		  managementService.activateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		  fail("Exception expected: It should not be possible to activate a job");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(userId, message);
		  assertTextPresent(UPDATE.Name, message);
		  assertTextPresent(PROCESS_INSTANCE.resourceName(), message);
		  assertTextPresent(UPDATE_INSTANCE.Name, message);
		  assertTextPresent(TIMER_BOUNDARY_PROCESS_KEY, message);
		  assertTextPresent(PROCESS_DEFINITION.resourceName(), message);
		}
	  }

	  public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessInstance()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_INSTANCE, ANY, userId, UPDATE);

		// when
		managementService.activateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, TIMER_BOUNDARY_PROCESS_KEY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  public virtual void testActivateJobByProcessDefinitionKeyWihtUpdatePermissionOnAnyProcessDefinition()
	  {
		// given
		string processInstanceId = startProcessInstanceByKey(TIMER_BOUNDARY_PROCESS_KEY).Id;
		suspendJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);
		createGrantAuthorization(PROCESS_DEFINITION, ANY, userId, UPDATE_INSTANCE);

		// when
		managementService.activateJobByProcessDefinitionKey(TIMER_BOUNDARY_PROCESS_KEY);

		// then
		Job job = selectJobByProcessInstanceId(processInstanceId);
		assertNotNull(job);
		assertFalse(job.Suspended);
	  }

	  // helper /////////////////////////////////////////////////////

	  protected internal virtual void verifyQueryResults(JobQuery query, int countExpected)
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: verifyQueryResults((org.camunda.bpm.engine.impl.AbstractQuery<?, ?>) query, countExpected);
		verifyQueryResults((AbstractQuery<object, ?>) query, countExpected);
	  }

	  protected internal virtual Job selectAnyJob()
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().listPage(0, 1).get(0);
		enableAuthorization();
		return job;
	  }

	  protected internal virtual void deleteJob(string jobId)
	  {
		disableAuthorization();
		managementService.deleteJob(jobId);
		enableAuthorization();
	  }

	  protected internal virtual Job selectJobByProcessInstanceId(string processInstanceId)
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().processInstanceId(processInstanceId).singleResult();
		enableAuthorization();
		return job;
	  }

	  protected internal virtual Job selectJobById(string jobId)
	  {
		disableAuthorization();
		Job job = managementService.createJobQuery().jobId(jobId).singleResult();
		enableAuthorization();
		return job;
	  }

	  protected internal virtual JobDefinition selectJobDefinitionByProcessDefinitionKey(string processDefinitionKey)
	  {
		disableAuthorization();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().processDefinitionKey(processDefinitionKey).singleResult();
		enableAuthorization();
		return jobDefinition;
	  }

	}

}