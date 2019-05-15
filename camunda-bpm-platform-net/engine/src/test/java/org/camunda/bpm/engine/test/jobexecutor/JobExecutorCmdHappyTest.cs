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

	using HistoricJobLog = org.camunda.bpm.engine.history.HistoricJobLog;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using ExecuteJobHelper = org.camunda.bpm.engine.impl.jobexecutor.ExecuteJobHelper;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using TimerEntity = org.camunda.bpm.engine.impl.persistence.entity.TimerEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;

	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JobExecutorCmdHappyTest : JobExecutorTestCase
	{

	  public virtual void testJobCommandsWithMessage()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;
		string jobId = commandExecutor.execute(new CommandAnonymousInnerClass(this));

		AcquiredJobs acquiredJobs = commandExecutor.execute(new AcquireJobsCmd(jobExecutor));
		IList<IList<string>> jobIdsList = acquiredJobs.JobIdBatches;
		assertEquals(1, jobIdsList.Count);

		IList<string> jobIds = jobIdsList[0];

		IList<string> expectedJobIds = new List<string>();
		expectedJobIds.Add(jobId);

		assertEquals(expectedJobIds, new List<string>(jobIds));
		assertEquals(0, tweetHandler.Messages.Count);

		ExecuteJobHelper.executeJob(jobId, commandExecutor);

		assertEquals("i'm coding a test", tweetHandler.Messages[0]);
		assertEquals(1, tweetHandler.Messages.Count);

		clearDatabase();
	  }

	  private class CommandAnonymousInnerClass : Command<string>
	  {
		  private readonly JobExecutorCmdHappyTest outerInstance;

		  public CommandAnonymousInnerClass(JobExecutorCmdHappyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public string execute(CommandContext commandContext)
		  {
			MessageEntity message = outerInstance.createTweetMessage("i'm coding a test");
			commandContext.JobManager.send(message);
			return message.Id;
		  }
	  }

	  internal const long SOME_TIME = 928374923546L;
	  internal const long SECOND = 1000;

	  public virtual void testJobCommandsWithTimer()
	  {
		// clock gets automatically reset in LogTestCase.runTest
		ClockUtil.CurrentTime = new DateTime(SOME_TIME);

		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		JobExecutor jobExecutor = processEngineConfiguration.JobExecutor;

		string jobId = commandExecutor.execute(new CommandAnonymousInnerClass2(this));

		AcquiredJobs acquiredJobs = commandExecutor.execute(new AcquireJobsCmd(jobExecutor));
		IList<IList<string>> jobIdsList = acquiredJobs.JobIdBatches;
		assertEquals(0, jobIdsList.Count);

		IList<string> expectedJobIds = new List<string>();

		ClockUtil.CurrentTime = new DateTime(SOME_TIME + (20 * SECOND));

		acquiredJobs = commandExecutor.execute(new AcquireJobsCmd(jobExecutor, jobExecutor.MaxJobsPerAcquisition));
		jobIdsList = acquiredJobs.JobIdBatches;
		assertEquals(1, jobIdsList.Count);

		IList<string> jobIds = jobIdsList[0];

		expectedJobIds.Add(jobId);
		assertEquals(expectedJobIds, new List<string>(jobIds));

		assertEquals(0, tweetHandler.Messages.Count);

		ExecuteJobHelper.executeJob(jobId, commandExecutor);

		assertEquals("i'm coding a test", tweetHandler.Messages[0]);
		assertEquals(1, tweetHandler.Messages.Count);

		clearDatabase();
	  }

	  private class CommandAnonymousInnerClass2 : Command<string>
	  {
		  private readonly JobExecutorCmdHappyTest outerInstance;

		  public CommandAnonymousInnerClass2(JobExecutorCmdHappyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


		  public string execute(CommandContext commandContext)
		  {
			TimerEntity timer = outerInstance.createTweetTimer("i'm coding a test", new DateTime(SOME_TIME + (10 * SECOND)));
			commandContext.JobManager.schedule(timer);
			return timer.Id;
		  }
	  }

	  protected internal virtual void clearDatabase()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass3(this));
	  }

	  private class CommandAnonymousInnerClass3 : Command<Void>
	  {
		  private readonly JobExecutorCmdHappyTest outerInstance;

		  public CommandAnonymousInnerClass3(JobExecutorCmdHappyTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

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