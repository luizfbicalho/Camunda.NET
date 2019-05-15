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
namespace org.camunda.bpm.engine.test.api.mgmt
{
	using NotFoundException = org.camunda.bpm.engine.exception.NotFoundException;
	using NullValueException = org.camunda.bpm.engine.exception.NullValueException;
	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using HistoricIncidentEntity = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentEntity;
	using HistoricIncidentManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using JobDefinition = org.camunda.bpm.engine.management.JobDefinition;
	using TableMetaData = org.camunda.bpm.engine.management.TableMetaData;
	using TablePage = org.camunda.bpm.engine.management.TablePage;
	using Incident = org.camunda.bpm.engine.runtime.Incident;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using Assert = org.junit.Assert;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	/// <summary>
	/// @author Frederik Heremans
	/// @author Falko Menge
	/// @author Saeid Mizaei
	/// @author Joram Barrez
	/// </summary>
	public class ManagementServiceTest : PluggableProcessEngineTestCase
	{

	  public virtual void testGetMetaDataForUnexistingTable()
	  {
		TableMetaData metaData = managementService.getTableMetaData("unexistingtable");
		assertNull(metaData);
	  }

	  public virtual void testGetMetaDataNullTableName()
	  {
		try
		{
		  managementService.getTableMetaData(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("tableName is null", re.Message);
		}
	  }

	  public virtual void testExecuteJobNullJobId()
	  {
		try
		{
		  managementService.executeJob(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("jobId is null", re.Message);
		}
	  }

	  public virtual void testExecuteJobUnexistingJob()
	  {
		try
		{
		  managementService.executeJob("unexistingjob");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("No job found with id", ae.Message);
		}
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testGetJobExceptionStacktrace()
	  public virtual void testGetJobExceptionStacktrace()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundry
		// timer event which we will execute manual for testing purposes.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);

		try
		{
		  managementService.executeJob(timerJob.Id);
		  fail("RuntimeException from within the script task expected");
		}
		catch (Exception re)
		{
		  assertTextPresent("This is an exception thrown from scriptTask", re.Message);
		}

		// Fetch the task to see that the exception that occurred is persisted
		timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		Assert.assertNotNull(timerJob);
		Assert.assertNotNull(timerJob.ExceptionMessage);
		assertTextPresent("This is an exception thrown from scriptTask", timerJob.ExceptionMessage);

		// Get the full stacktrace using the managementService
		string exceptionStack = managementService.getJobExceptionStacktrace(timerJob.Id);
		Assert.assertNotNull(exceptionStack);
		assertTextPresent("This is an exception thrown from scriptTask", exceptionStack);
	  }

	  public virtual void testgetJobExceptionStacktraceUnexistingJobId()
	  {
		try
		{
		  managementService.getJobExceptionStacktrace("unexistingjob");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("No job found with id unexistingjob", re.Message);
		}
	  }

	  public virtual void testgetJobExceptionStacktraceNullJobId()
	  {
		try
		{
		  managementService.getJobExceptionStacktrace(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("jobId is null", re.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobRetries()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundary
		// timer event.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);
		assertEquals(JobEntity.DEFAULT_RETRIES, timerJob.Retries);

		managementService.setJobRetries(timerJob.Id, 5);

		timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals(5, timerJob.Retries);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobsRetries()
	  {
		//given
		runtimeService.startProcessInstanceByKey("exceptionInJobExecution");
		runtimeService.startProcessInstanceByKey("exceptionInJobExecution");
		IList<string> allJobIds = AllJobIds;

		//when
		managementService.setJobRetries(allJobIds, 5);

		//then
		assertRetries(allJobIds, 5);
	  }

	  public virtual void testSetJobsRetriesWithNull()
	  {
		try
		{
		  //when
		  managementService.setJobRetries((IList<string>) null, 5);
		  fail("exception expected");
		  //then
		}
		catch (ProcessEngineException)
		{
		  //expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobsRetriesWithNegativeRetries()
	  {
		try
		{
		  //when
		  managementService.setJobRetries(Arrays.asList(new string[]{"aFake"}), -1);
		  fail("exception expected");
		  //then
		}
		catch (ProcessEngineException)
		{
		  //expected
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobsRetriesWithFake()
	  {
		//given
		runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		IList<string> allJobIds = AllJobIds;
		allJobIds.Add("aFake");
		try
		{
		  //when
		  managementService.setJobRetries(allJobIds, 5);
		  fail("exception expected");
		  //then
		}
		catch (ProcessEngineException)
		{
		  //expected
		}

		assertRetries(AllJobIds, JobEntity.DEFAULT_RETRIES);
	  }

	  protected internal virtual void assertRetries(IList<string> allJobIds, int i)
	  {
		foreach (string id in allJobIds)
		{
		  assertThat(managementService.createJobQuery().jobId(id).singleResult().Retries, @is(i));
		}
	  }

	  protected internal virtual IList<string> AllJobIds
	  {
		  get
		  {
			List<string> result = new List<string>();
			foreach (Job job in managementService.createJobQuery().list())
			{
			  result.Add(job.Id);
			}
			return result;
		  }
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobRetriesNullCreatesIncident()
	  {

		// initially there is no incident
		assertEquals(0, runtimeService.createIncidentQuery().count());

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundary
		// timer event.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);
		assertEquals(JobEntity.DEFAULT_RETRIES, timerJob.Retries);

		managementService.setJobRetries(timerJob.Id, 0);

		timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertEquals(0, timerJob.Retries);

		assertEquals(1, runtimeService.createIncidentQuery().count());

	  }

	  public virtual void testSetJobRetriesUnexistingJobId()
	  {
		try
		{
		  managementService.setJobRetries("unexistingjob", 5);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("No job found with id 'unexistingjob'.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesEmptyJobId()
	  {
		try
		{
		  managementService.setJobRetries("", 5);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesJobIdNull()
	  {
		try
		{
		  managementService.setJobRetries((string) null, 5);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesNegativeNumberOfRetries()
	  {
		try
		{
		  managementService.setJobRetries("unexistingjob", -1);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("The number of job retries must be a non-negative Integer, but '-1' has been provided.", re.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobRetriesByJobDefinitionId()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");
		executeAvailableJobs();

		JobQuery query = managementService.createJobQuery().processInstanceId(processInstance.Id);

		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		Job timerJob = query.singleResult();

		assertNotNull("No job found for process instance", timerJob);
		assertEquals(0, timerJob.Retries);

		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 5);

		timerJob = query.singleResult();
		assertEquals(5, timerJob.Retries);
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdEmptyJobDefinitionId()
	  {
		try
		{
		  managementService.setJobRetriesByJobDefinitionId("", 5);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdNull()
	  {
		try
		{
		  managementService.setJobRetriesByJobDefinitionId(null, 5);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("Either job definition id or job id has to be provided as parameter.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesByJobDefinitionIdNegativeNumberOfRetries()
	  {
		try
		{
		  managementService.setJobRetries("unexistingjob", -1);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("The number of job retries must be a non-negative Integer, but '-1' has been provided.", re.Message);
		}
	  }

	  public virtual void testSetJobRetriesUnlocksInconsistentJob()
	  {
		// case 1
		// given an inconsistent job that is never again picked up by a job executor
		createJob(0, "owner", ClockUtil.CurrentTime);

		// when the job retries are reset
		JobEntity job = (JobEntity) managementService.createJobQuery().singleResult();
		managementService.setJobRetries(job.Id, 3);

		// then the job can be picked up again
		job = (JobEntity) managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertNull(job.LockOwner);
		assertNull(job.LockExpirationTime);
		assertEquals(3, job.Retries);

		deleteJobAndIncidents(job);

		// case 2
		// given an inconsistent job that is never again picked up by a job executor
		createJob(2, "owner", null);

		// when the job retries are reset
		job = (JobEntity) managementService.createJobQuery().singleResult();
		managementService.setJobRetries(job.Id, 3);

		// then the job can be picked up again
		job = (JobEntity) managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertNull(job.LockOwner);
		assertNull(job.LockExpirationTime);
		assertEquals(3, job.Retries);

		deleteJobAndIncidents(job);

		// case 3
		// given a consistent job
		createJob(2, "owner", ClockUtil.CurrentTime);

		// when the job retries are reset
		job = (JobEntity) managementService.createJobQuery().singleResult();
		managementService.setJobRetries(job.Id, 3);

		// then the lock owner and expiration should not change
		job = (JobEntity) managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertNotNull(job.LockOwner);
		assertNotNull(job.LockExpirationTime);
		assertEquals(3, job.Retries);

		deleteJobAndIncidents(job);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void createJob(final int retries, final String owner, final java.util.Date lockExpirationTime)
	  protected internal virtual void createJob(int retries, string owner, DateTime lockExpirationTime)
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this, retries, owner, lockExpirationTime));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly ManagementServiceTest outerInstance;

		  private int retries;
		  private string owner;
		  private DateTime lockExpirationTime;

		  public CommandAnonymousInnerClass(ManagementServiceTest outerInstance, int retries, string owner, DateTime lockExpirationTime)
		  {
			  this.outerInstance = outerInstance;
			  this.retries = retries;
			  this.owner = owner;
			  this.lockExpirationTime = lockExpirationTime;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobManager jobManager = commandContext.JobManager;
			MessageEntity job = new MessageEntity();
			job.JobHandlerType = "any";
			job.LockOwner = owner;
			job.LockExpirationTime = lockExpirationTime;
			job.Retries = retries;

			jobManager.send(job);
			return null;
		  }
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobRetriesByDefinitionUnlocksInconsistentJobs()
	  {
		// given a job definition
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.management.JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();
		JobDefinition jobDefinition = managementService.createJobDefinitionQuery().singleResult();

		// and an inconsistent job that is never again picked up by a job executor
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass2(this, jobDefinition));

		// when the job retries are reset
		managementService.setJobRetriesByJobDefinitionId(jobDefinition.Id, 3);

		// then the job can be picked up again
		JobEntity job = (JobEntity) managementService.createJobQuery().singleResult();
		assertNotNull(job);
		assertNull(job.LockOwner);
		assertNull(job.LockExpirationTime);
		assertEquals(3, job.Retries);

		deleteJobAndIncidents(job);
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly ManagementServiceTest outerInstance;

		  private JobDefinition jobDefinition;

		  public CommandAnonymousInnerClass2(ManagementServiceTest outerInstance, JobDefinition jobDefinition)
		  {
			  this.outerInstance = outerInstance;
			  this.jobDefinition = jobDefinition;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobManager jobManager = commandContext.JobManager;
			MessageEntity job = new MessageEntity();
			job.JobDefinitionId = jobDefinition.Id;
			job.JobHandlerType = "any";
			job.LockOwner = "owner";
			job.LockExpirationTime = ClockUtil.CurrentTime;
			job.Retries = 0;

			jobManager.send(job);
			return null;
		  }
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void deleteJobAndIncidents(final org.camunda.bpm.engine.runtime.Job job)
	  protected internal virtual void deleteJobAndIncidents(Job job)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.List<org.camunda.bpm.engine.history.HistoricIncident> incidents = historyService.createHistoricIncidentQuery().incidentType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE).list();
		IList<HistoricIncident> incidents = historyService.createHistoricIncidentQuery().incidentType(org.camunda.bpm.engine.runtime.Incident_Fields.FAILED_JOB_HANDLER_TYPE).list();

		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass3(this, job, incidents));
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly ManagementServiceTest outerInstance;

		  private Job job;
		  private IList<HistoricIncident> incidents;

		  public CommandAnonymousInnerClass3(ManagementServiceTest outerInstance, Job job, IList<HistoricIncident> incidents)
		  {
			  this.outerInstance = outerInstance;
			  this.job = job;
			  this.incidents = incidents;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			((JobEntity) job).delete();

			HistoricIncidentManager historicIncidentManager = commandContext.HistoricIncidentManager;
			foreach (HistoricIncident incident in incidents)
			{
			  HistoricIncidentEntity incidentEntity = (HistoricIncidentEntity) incident;
			  historicIncidentManager.delete(incidentEntity);
			}

			commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			return null;
		  }
	  }

	  public virtual void testDeleteJobNullJobId()
	  {
		try
		{
		  managementService.deleteJob(null);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("jobId is null", re.Message);
		}
	  }

	  public virtual void testDeleteJobUnexistingJob()
	  {
		try
		{
		  managementService.deleteJob("unexistingjob");
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException ae)
		{
		  assertTextPresent("No job found with id", ae.Message);
		}
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/timerOnTask.bpmn20.xml"})]
	  public virtual void testDeleteJobDeletion()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("timerOnTask");
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("Task timer should be there", timerJob);
		managementService.deleteJob(timerJob.Id);

		timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();
		assertNull("There should be no job now. It was deleted", timerJob);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/timerOnTask.bpmn20.xml"})]
	  public virtual void testDeleteJobThatWasAlreadyAcquired()
	  {
		ClockUtil.CurrentTime = DateTime.Now;

		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("timerOnTask");
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		// We need to move time at least one hour to make the timer executable
		ClockUtil.CurrentTime = new DateTime(ClockUtil.CurrentTime.Ticks + 7200000L);

		// Acquire job by running the acquire command manually
		ProcessEngineImpl processEngineImpl = (ProcessEngineImpl) processEngine;
		JobExecutor jobExecutor = processEngineImpl.ProcessEngineConfiguration.JobExecutor;
		AcquireJobsCmd acquireJobsCmd = new AcquireJobsCmd(jobExecutor);
		CommandExecutor commandExecutor = processEngineImpl.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(acquireJobsCmd);

		// Try to delete the job. This should fail.
		try
		{
		  managementService.deleteJob(timerJob.Id);
		  fail();
		}
		catch (ProcessEngineException)
		{
		  // Exception is expected
		}

		// Clean up
		managementService.executeJob(timerJob.Id);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobDuedate()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundary
		// timer event.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);
		assertNotNull(timerJob.Duedate);

		DateTime cal = new DateTime();
		cal = new DateTime(DateTime.Now);
		cal.AddDays(3); // add 3 days on the actual date
		managementService.setJobDuedate(timerJob.Id, cal);

		Job newTimerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		// normalize date for mysql dropping fractional seconds in time values
		int SECOND = 1000;
		assertEquals((cal.Ticks / SECOND) * SECOND, (newTimerJob.Duedate.Ticks / SECOND) * SECOND);
	  }

	  [Deployment(resources : {"org/camunda/bpm/engine/test/api/mgmt/ManagementServiceTest.testGetJobExceptionStacktrace.bpmn20.xml"})]
	  public virtual void testSetJobDuedateDateNull()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("exceptionInJobExecution");

		// The execution is waiting in the first usertask. This contains a boundary
		// timer event.
		Job timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNotNull("No job found for process instance", timerJob);
		assertNotNull(timerJob.Duedate);

		managementService.setJobDuedate(timerJob.Id, null);

		timerJob = managementService.createJobQuery().processInstanceId(processInstance.Id).singleResult();

		assertNull(timerJob.Duedate);
	  }


	  public virtual void testSetJobDuedateJobIdNull()
	  {
		try
		{
		  managementService.setJobDuedate(null, DateTime.Now);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("The job id is mandatory, but 'null' has been provided.", re.Message);
		}
	  }

	  public virtual void testSetJobDuedateEmptyJobId()
	  {
		try
		{
		  managementService.setJobDuedate("", DateTime.Now);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("The job id is mandatory, but '' has been provided.", re.Message);
		}
	  }

	  public virtual void testSetJobDuedateUnexistingJobId()
	  {
		try
		{
		  managementService.setJobDuedate("unexistingjob", DateTime.Now);
		  fail("ProcessEngineException expected");
		}
		catch (ProcessEngineException re)
		{
		  assertTextPresent("No job found with id 'unexistingjob'.", re.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/bpmn/job/oneTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobDuedateNonTimerJob()
	  {
		runtimeService.startProcessInstanceByKey("oneTaskProcess");
		Job job = managementService.createJobQuery().processDefinitionKey("oneTaskProcess").singleResult();
		assertNotNull(job);
		managementService.setJobDuedate(job.Id, DateTime.Now);
		job = managementService.createJobQuery().processDefinitionKey("oneTaskProcess").singleResult();
		assertNotNull(job.Duedate);
	  }

	  public virtual void testGetProperties()
	  {
		IDictionary<string, string> properties = managementService.Properties;
		assertNotNull(properties);
		assertFalse(properties.Count == 0);
	  }

	  public virtual void testSetProperty()
	  {
		const string name = "testProp";
		const string value = "testValue";
		managementService.setProperty(name, value);

		IDictionary<string, string> properties = managementService.Properties;
		assertTrue(properties.ContainsKey(name));
		string storedValue = properties[name];
		assertEquals(value, storedValue);

		managementService.deleteProperty(name);
	  }

	  public virtual void testDeleteProperty()
	  {
		const string name = "testProp";
		const string value = "testValue";
		managementService.setProperty(name, value);

		IDictionary<string, string> properties = managementService.Properties;
		assertTrue(properties.ContainsKey(name));
		string storedValue = properties[name];
		assertEquals(value, storedValue);

		managementService.deleteProperty(name);
		properties = managementService.Properties;
		assertFalse(properties.ContainsKey(name));

	  }

	  public virtual void testDeleteNonexistingProperty()
	  {

		managementService.deleteProperty("non existing");

	  }

	  public virtual void testGetHistoryLevel()
	  {
		int historyLevel = managementService.HistoryLevel;
		assertEquals(processEngineConfiguration.HistoryLevel.Id, historyLevel);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobPriority()
	  {
		// given
		runtimeService.createProcessInstanceByKey("asyncTaskProcess").startBeforeActivity("task").execute();

		Job job = managementService.createJobQuery().singleResult();

		// when
		managementService.setJobPriority(job.Id, 42);

		// then
		job = managementService.createJobQuery().singleResult();

		assertEquals(42, job.Priority);
	  }

	  public virtual void testSetJobPriorityForNonExistingJob()
	  {
		try
		{
		  managementService.setJobPriority("nonExistingJob", 42);
		  fail("should not succeed");
		}
		catch (NotFoundException e)
		{
		  assertTextPresentIgnoreCase("No job found with id 'nonExistingJob'", e.Message);
		}
	  }

	  public virtual void testSetJobPriorityForNullJob()
	  {
		try
		{
		  managementService.setJobPriority(null, 42);
		  fail("should not succeed");
		}
		catch (NullValueException e)
		{
		  assertTextPresentIgnoreCase("Job id must not be null", e.Message);
		}
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/asyncTaskProcess.bpmn20.xml")]
	  public virtual void testSetJobPriorityToExtremeValues()
	  {
		runtimeService.createProcessInstanceByKey("asyncTaskProcess").startBeforeActivity("task").execute();

		Job job = managementService.createJobQuery().singleResult();

		// it is possible to set the max integer value
		managementService.setJobPriority(job.Id, long.MaxValue);
		job = managementService.createJobQuery().singleResult();
		assertEquals(long.MaxValue, job.Priority);

		// it is possible to set the min integer value
		managementService.setJobPriority(job.Id, long.MinValue + 1); // +1 for informix
		job = managementService.createJobQuery().singleResult();
		assertEquals(long.MinValue + 1, job.Priority);
	  }

	  public virtual void testGetTableMetaData()
	  {

		TableMetaData tableMetaData = managementService.getTableMetaData("ACT_RU_TASK");
		assertEquals(tableMetaData.ColumnNames.Count, tableMetaData.ColumnTypes.Count);
		assertEquals(21, tableMetaData.ColumnNames.Count);

		int assigneeIndex = tableMetaData.ColumnNames.IndexOf("ASSIGNEE_");
		int createTimeIndex = tableMetaData.ColumnNames.IndexOf("CREATE_TIME_");

		assertTrue(assigneeIndex >= 0);
		assertTrue(createTimeIndex >= 0);

		assertOneOf(new string[]{"VARCHAR", "NVARCHAR2", "nvarchar", "NVARCHAR"}, tableMetaData.ColumnTypes[assigneeIndex]);
		assertOneOf(new string[]{"TIMESTAMP", "TIMESTAMP(6)", "datetime", "DATETIME", "DATETIME2"}, tableMetaData.ColumnTypes[createTimeIndex]);
	  }

	  private void assertOneOf(string[] possibleValues, string currentValue)
	  {
		foreach (string value in possibleValues)
		{
		  if (currentValue.Equals(value))
		  {
			return;
		  }
		}
		fail("Value '" + currentValue + "' should be one of: " + Arrays.deepToString(possibleValues));
	  }

	  public virtual void testGetTablePage()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		IList<string> taskIds = generateDummyTasks(20);

		TablePage tablePage = managementService.createTablePageQuery().tableName(tablePrefix + "ACT_RU_TASK").listPage(0, 5);

		assertEquals(0, tablePage.FirstResult);
		assertEquals(5, tablePage.Size);
		assertEquals(5, tablePage.Rows.Count);
		assertEquals(20, tablePage.Total);

		tablePage = managementService.createTablePageQuery().tableName(tablePrefix + "ACT_RU_TASK").listPage(14, 10);

		assertEquals(14, tablePage.FirstResult);
		assertEquals(6, tablePage.Size);
		assertEquals(6, tablePage.Rows.Count);
		assertEquals(20, tablePage.Total);

		taskService.deleteTasks(taskIds, true);
	  }

	  public virtual void testGetSortedTablePage()
	  {
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;
		IList<string> taskIds = generateDummyTasks(15);

		// With an ascending sort
		TablePage tablePage = managementService.createTablePageQuery().tableName(tablePrefix + "ACT_RU_TASK").orderAsc("NAME_").listPage(1, 7);
		string[] expectedTaskNames = new string[]{"B", "C", "D", "E", "F", "G", "H"};
		verifyTaskNames(expectedTaskNames, tablePage.Rows);

		// With a descending sort
		tablePage = managementService.createTablePageQuery().tableName(tablePrefix + "ACT_RU_TASK").orderDesc("NAME_").listPage(6, 8);
		expectedTaskNames = new string[]{"I", "H", "G", "F", "E", "D", "C", "B"};
		verifyTaskNames(expectedTaskNames, tablePage.Rows);

		taskService.deleteTasks(taskIds, true);
	  }

	  private void verifyTaskNames(string[] expectedTaskNames, IList<IDictionary<string, object>> rowData)
	  {
		assertEquals(expectedTaskNames.Length, rowData.Count);
		string columnKey = "NAME_";

		for (int i = 0; i < expectedTaskNames.Length; i++)
		{
		  object o = rowData[i][columnKey];
		  if (o == null)
		  {
			o = rowData[i][columnKey.ToLower()];
		  }
		  assertEquals(expectedTaskNames[i], o);
		}
	  }

	  private IList<string> generateDummyTasks(int nrOfTasks)
	  {
		List<string> taskIds = new List<string>();
		for (int i = 0; i < nrOfTasks; i++)
		{
		  Task task = taskService.newTask();
		  task.Name = ((char)('A' + i)) + "";
		  taskService.saveTask(task);
		  taskIds.Add(task.Id);
		}
		return taskIds;
	  }

	}

}