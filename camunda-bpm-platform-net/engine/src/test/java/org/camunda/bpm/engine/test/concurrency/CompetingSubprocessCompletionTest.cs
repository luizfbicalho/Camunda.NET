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
namespace org.camunda.bpm.engine.test.concurrency
{

	using ProcessEngineLogger = org.camunda.bpm.engine.impl.ProcessEngineLogger;
	using CompleteTaskCmd = org.camunda.bpm.engine.impl.cmd.CompleteTaskCmd;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Task = org.camunda.bpm.engine.task.Task;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CompetingSubprocessCompletionTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal static ControllableThread activeThread;

	  public class CompleteTaskThread : ControllableThread
	  {
		  private readonly CompetingSubprocessCompletionTest outerInstance;

		internal string taskId;
		internal OptimisticLockingException exception;
		public CompleteTaskThread(CompetingSubprocessCompletionTest outerInstance, string taskId)
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

	  /// <summary>
	  /// This test requires a minimum of three concurrent executions to avoid
	  /// that all threads attempt compaction by which synchronization happens "by accident"
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompetingSubprocessEnd() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCompetingSubprocessEnd()
	  {
		runtimeService.startProcessInstanceByKey("CompetingSubprocessEndProcess");

		IList<Task> tasks = taskService.createTaskQuery().list();
		assertEquals(3, tasks.Count);

		LOG.debug("test thread starts thread one");
		CompleteTaskThread threadOne = new CompleteTaskThread(this, tasks[0].Id);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		CompleteTaskThread threadTwo = new CompleteTaskThread(this, tasks[1].Id);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		assertNotNull(threadTwo.exception);
		assertTextPresent("was updated by another transaction concurrently", threadTwo.exception.Message);
	  }

	}

}