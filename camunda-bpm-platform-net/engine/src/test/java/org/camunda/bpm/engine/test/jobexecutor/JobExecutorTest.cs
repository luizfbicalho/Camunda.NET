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
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class JobExecutorTest : JobExecutorTestCase
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void testBasicJobExecutorOperation() throws Exception
	  public virtual void testBasicJobExecutorOperation()
	  {
		CommandExecutor commandExecutor = processEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass(this));

		executeAvailableJobs();

		ISet<string> messages = new HashSet<string>(tweetHandler.Messages);
		ISet<string> expectedMessages = new HashSet<string>();
		expectedMessages.Add("message-one");
		expectedMessages.Add("message-two");
		expectedMessages.Add("message-three");
		expectedMessages.Add("message-four");
		expectedMessages.Add("timer-one");
		expectedMessages.Add("timer-two");

		assertEquals(new SortedSet<string>(expectedMessages), new SortedSet<string>(messages));

		commandExecutor.execute(new CommandAnonymousInnerClass2(this));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly JobExecutorTest outerInstance;

		  public CommandAnonymousInnerClass(JobExecutorTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			JobManager jobManager = commandContext.JobManager;
			jobManager.send(outerInstance.createTweetMessage("message-one"));
			jobManager.send(outerInstance.createTweetMessage("message-two"));
			jobManager.send(outerInstance.createTweetMessage("message-three"));
			jobManager.send(outerInstance.createTweetMessage("message-four"));

			jobManager.schedule(outerInstance.createTweetTimer("timer-one", DateTime.Now));
			jobManager.schedule(outerInstance.createTweetTimer("timer-two", DateTime.Now));
			return null;
		  }
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly JobExecutorTest outerInstance;

		  public CommandAnonymousInnerClass2(JobExecutorTest outerInstance)
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

	  public virtual void testJobExecutorHintConfiguration()
	  {
		ProcessEngineConfiguration engineConfig1 = ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration();

		assertTrue("default setting is true", engineConfig1.HintJobExecutor);

		ProcessEngineConfiguration engineConfig2 = ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration().setHintJobExecutor(false);

		assertFalse(engineConfig2.HintJobExecutor);

		ProcessEngineConfiguration engineConfig3 = ProcessEngineConfiguration.createStandaloneInMemProcessEngineConfiguration().setHintJobExecutor(true);

		assertTrue(engineConfig3.HintJobExecutor);
	  }

	  public virtual void testAcquiredJobs()
	  {
		IList<string> firstBatch = new List<string>(Arrays.asList("a", "b", "c"));
		IList<string> secondBatch = new List<string>(Arrays.asList("d", "e", "f"));
		IList<string> thirdBatch = new List<string>(Arrays.asList("g"));

		AcquiredJobs acquiredJobs = new AcquiredJobs(0);
		acquiredJobs.addJobIdBatch(firstBatch);
		acquiredJobs.addJobIdBatch(secondBatch);
		acquiredJobs.addJobIdBatch(thirdBatch);

		assertEquals(firstBatch, acquiredJobs.JobIdBatches[0]);
		assertEquals(secondBatch, acquiredJobs.JobIdBatches[1]);
		assertEquals(thirdBatch, acquiredJobs.JobIdBatches[2]);

		acquiredJobs.removeJobId("a");
		assertEquals(Arrays.asList("b", "c"), acquiredJobs.JobIdBatches[0]);
		assertEquals(secondBatch, acquiredJobs.JobIdBatches[1]);
		assertEquals(thirdBatch, acquiredJobs.JobIdBatches[2]);

		assertEquals(3, acquiredJobs.JobIdBatches.Count);
		acquiredJobs.removeJobId("g");
		assertEquals(2, acquiredJobs.JobIdBatches.Count);
	  }
	}

}