using System.Threading;

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
namespace org.camunda.bpm.engine.test.concurrency
{
	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using TaskQuery = org.camunda.bpm.engine.task.TaskQuery;
	using Ignore = org.junit.Ignore;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Ignore public class CompetingForkTest extends org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase
	public class CompetingForkTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;
	  internal static string jobId;

	  public class CompleteTaskThread : ControllableThread
	  {
		  private readonly CompetingForkTest outerInstance;


		internal string taskId;
		internal OptimisticLockingException exception;

		public CompleteTaskThread(CompetingForkTest outerInstance, string taskId)
		{
			this.outerInstance = outerInstance;
		  this.taskId = taskId;
		}
		public override void startAndWaitUntilControlIsReturned()
		{
			lock (this)
			{
			  activeThread = this;
			  base.startAndWaitUntilControlIsReturned();
			}
		}

		public virtual void run()
		{
		  try
		  {
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new CompleteTaskCmd(taskId, null)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void FAILING_testCompetingFork() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void FAILING_testCompetingFork()
	  {
		runtimeService.startProcessInstanceByKey("process");

		TaskQuery query = taskService.createTaskQuery();

		string task1 = query.taskDefinitionKey("task1").singleResult().Id;

		string task2 = query.taskDefinitionKey("task2").singleResult().Id;

		string task3 = query.taskDefinitionKey("task3").singleResult().Id;

		LOG.debug("test thread starts thread one");
		CompleteTaskThread threadOne = new CompleteTaskThread(this, task1);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread thread two");
		CompleteTaskThread threadTwo = new CompleteTaskThread(this, task2);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread three");
		CompleteTaskThread threadThree = new CompleteTaskThread(this, task3);
		threadThree.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);
		assertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);

		LOG.debug("test thread notifies thread 3");
		threadThree.proceedAndWaitTillDone();
		assertNotNull(threadThree.exception);
		assertTextPresent("was updated by another transaction concurrently", threadThree.exception.Message);
	  }
	}

}