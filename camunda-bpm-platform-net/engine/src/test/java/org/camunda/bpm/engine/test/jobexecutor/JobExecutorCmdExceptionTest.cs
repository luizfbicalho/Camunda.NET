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
namespace org.camunda.bpm.engine.test.jobexecutor
{

	using HistoricIncident = org.camunda.bpm.engine.history.HistoricIncident;
	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using DeleteJobCmd = org.camunda.bpm.engine.impl.cmd.DeleteJobCmd;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using Bpmn = org.camunda.bpm.model.bpmn.Bpmn;

	/// <summary>
	/// @author Tom Baeyens
	/// @author Thorben Lindhauer
	/// </summary>
	public class JobExecutorCmdExceptionTest : PluggableProcessEngineTestCase
	{

	  protected internal TweetExceptionHandler tweetExceptionHandler = new TweetExceptionHandler();
	  protected internal TweetNestedCommandExceptionHandler nestedCommandExceptionHandler = new TweetNestedCommandExceptionHandler();

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void setUp() throws Exception
	  public override void setUp()
	  {
		processEngineConfiguration.JobHandlers[tweetExceptionHandler.Type] = tweetExceptionHandler;
		processEngineConfiguration.JobHandlers[nestedCommandExceptionHandler.Type] = nestedCommandExceptionHandler;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void tearDown() throws Exception
	  public override void tearDown()
	  {
		processEngineConfiguration.JobHandlers.Remove(tweetExceptionHandler.Type);
		processEngineConfiguration.JobHandlers.Remove(nestedCommandExceptionHandler.Type);
		clearDatabase();
	  }

	  public virtual void testJobCommandsWith2Exceptions()
	  {
		// create a job
		createJob(TweetExceptionHandler.TYPE);

		// execute the existing job
		executeAvailableJobs();

		// the job was successfully executed
		JobQuery query = managementService.createJobQuery().noRetriesLeft();
		assertEquals(0, query.count());
	  }

	  public virtual void testJobCommandsWith3Exceptions()
	  {
		// set the execptionsRemaining to 3 so that
		// the created job will fail 3 times and a failed
		// job exists
		tweetExceptionHandler.ExceptionsRemaining = 3;

		// create a job
		createJob(TweetExceptionHandler.TYPE);

		// execute the existing job
		executeAvailableJobs();

		// the job execution failed (job.retries = 0)
		Job job = managementService.createJobQuery().noRetriesLeft().singleResult();
		assertNotNull(job);
		assertEquals(0, job.Retries);
	  }

	  public virtual void testMultipleFailingJobs()
	  {
		// set the execptionsRemaining to 600 so that
		// each created job will fail 3 times and 40 failed
		// job exists
		tweetExceptionHandler.ExceptionsRemaining = 600;

		// create 40 jobs
		for (int i = 0; i < 40; i++)
		{
		  createJob(TweetExceptionHandler.TYPE);
		}

		// execute the existing jobs
		executeAvailableJobs();

		// now there are 40 jobs with retries = 0:
		IList<Job> jobList = managementService.createJobQuery().list();
		assertEquals(40, jobList.Count);

		foreach (Job job in jobList)
		{
		  // all jobs have retries exhausted
		  assertEquals(0, job.Retries);
		}
	  }

	  public virtual void testJobCommandsWithNestedFailingCommand()
	  {
		// create a job
		createJob(TweetNestedCommandExceptionHandler.TYPE);

		// execute the existing job
		Job job = managementService.createJobQuery().singleResult();

		assertEquals(3, job.Retries);

		try
		{
		  managementService.executeJob(job.Id);
		  fail("Exception expected");
		}
		catch (Exception)
		{
		  // expected
		}

		job = managementService.createJobQuery().singleResult();
		assertEquals(2, job.Retries);

		executeAvailableJobs();

		// the job execution failed (job.retries = 0)
		job = managementService.createJobQuery().noRetriesLeft().singleResult();
		assertNotNull(job);
		assertEquals(0, job.Retries);
	  }

	  [Deployment(resources:"org/camunda/bpm/engine/test/jobexecutor/jobFailingOnFlush.bpmn20.xml")]
	  public virtual void testJobRetriesDecrementedOnFailedFlush()
	  {

		runtimeService.startProcessInstanceByKey("testProcess");

		// there should be 1 job created:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		// with 3 retries
		assertEquals(3, job.Retries);

		// if we execute the job
		waitForJobExecutorToProcessAllJobs(6000);

		// the job is still present
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		// but has no more retires
		assertEquals(0, job.Retries);
	  }

	  public virtual void testFailingTransactionListener()
	  {

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		deployment(Bpmn.createExecutableProcess("testProcess").startEvent().serviceTask().camundaClass(typeof(FailingTransactionListenerDelegate).FullName).camundaAsyncBefore().endEvent().done());

		runtimeService.startProcessInstanceByKey("testProcess");

		// there should be 1 job created:
		Job job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		// with 3 retries
		assertEquals(3, job.Retries);

		// if we execute the job
		waitForJobExecutorToProcessAllJobs(6000);

		// the job is still present
		job = managementService.createJobQuery().singleResult();
		assertNotNull(job);
		// but has no more retires
		assertEquals(0, job.Retries);
		assertEquals("exception in transaction listener", job.ExceptionMessage);

		string stacktrace = managementService.getJobExceptionStacktrace(job.Id);
		assertNotNull(stacktrace);
		assertTrue("unexpected stacktrace, was <" + stacktrace + ">", stacktrace.Contains("java.lang.RuntimeException: exception in transaction listener"));
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: protected void createJob(final String handlerType)
	  protected internal virtual void createJob(string handlerType)
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this, handlerType));
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly JobExecutorCmdExceptionTest outerInstance;

		  private string handlerType;

		  public CommandAnonymousInnerClass(JobExecutorCmdExceptionTest outerInstance, string handlerType)
		  {
			  this.outerInstance = outerInstance;
			  this.handlerType = handlerType;
		  }


		  public string execute(CommandContext commandContext)
		  {
			MessageEntity message = outerInstance.createMessage(handlerType);
			commandContext.JobManager.send(message);
			return message.Id;
		  }
	  }

	  protected internal virtual MessageEntity createMessage(string handlerType)
	  {
		MessageEntity message = new MessageEntity();
		message.JobHandlerType = handlerType;
		return message;
	  }

	  protected internal virtual void clearDatabase()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly JobExecutorCmdExceptionTest outerInstance;

		  public CommandAnonymousInnerClass2(JobExecutorCmdExceptionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			IList<Job> jobs = outerInstance.processEngineConfiguration.ManagementService.createJobQuery().list();

			foreach (Job job in jobs)
			{
			  (new DeleteJobCmd(job.Id)).execute(commandContext);
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogByJobId(job.Id);
			}

			IList<HistoricIncident> historicIncidents = outerInstance.processEngineConfiguration.HistoryService.createHistoricIncidentQuery().list();

			foreach (HistoricIncident historicIncident in historicIncidents)
			{
			  commandContext.DbEntityManager.delete((DbEntity) historicIncident);
			}

			IList<HistoricJobLog> historicJobLogs = outerInstance.processEngineConfiguration.HistoryService.createHistoricJobLogQuery().list();

			foreach (HistoricJobLog historicJobLog in historicJobLogs)
			{
			  commandContext.HistoricJobLogManager.deleteHistoricJobLogById(historicJobLog.Id);
			}

			return null;
		  }
	  }

	}

}