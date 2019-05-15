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

	using LockedExternalTask = org.camunda.bpm.engine.externaltask.LockedExternalTask;
	using FetchExternalTasksCmd = org.camunda.bpm.engine.impl.cmd.FetchExternalTasksCmd;
	using TopicFetchInstruction = org.camunda.bpm.engine.impl.externaltask.TopicFetchInstruction;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class CompetingExternalTaskFetchingTest : PluggableProcessEngineTestCase
	{

	  public class ExternalTaskFetcherThread : ControllableThread
	  {
		  private readonly CompetingExternalTaskFetchingTest outerInstance;


		protected internal string workerId;
		protected internal int results;
		protected internal string topic;

		protected internal IList<LockedExternalTask> fetchedTasks;
		protected internal OptimisticLockingException exception;

		public ExternalTaskFetcherThread(CompetingExternalTaskFetchingTest outerInstance, string workerId, int results, string topic)
		{
			this.outerInstance = outerInstance;
		  this.workerId = workerId;
		  this.results = results;
		  this.topic = topic;
		}

		public virtual void run()
		{
		  IDictionary<string, TopicFetchInstruction> instructions = new Dictionary<string, TopicFetchInstruction>();

		  TopicFetchInstruction instruction = new TopicFetchInstruction(topic, 10000L);
		  instructions[topic] = instruction;

		  try
		  {
			fetchedTasks = outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new FetchExternalTasksCmd(workerId, results, instructions));
		  }
		  catch (OptimisticLockingException e)
		  {
			exception = e;
		  }
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompetingExternalTaskFetching()
	  public virtual void testCompetingExternalTaskFetching()
	  {
		runtimeService.startProcessInstanceByKey("oneExternalTaskProcess");

		ExternalTaskFetcherThread thread1 = new ExternalTaskFetcherThread(this, "thread1", 5, "externalTaskTopic");
		ExternalTaskFetcherThread thread2 = new ExternalTaskFetcherThread(this, "thread2", 5, "externalTaskTopic");

		// both threads fetch the same task and wait before flushing the lock
		thread1.startAndWaitUntilControlIsReturned();
		thread2.startAndWaitUntilControlIsReturned();

		// thread1 succeeds
		thread1.proceedAndWaitTillDone();
		assertNull(thread1.exception);
		assertEquals(1, thread1.fetchedTasks.Count);

		// thread2 does not succeed in locking the job
		thread2.proceedAndWaitTillDone();
		assertEquals(0, thread2.fetchedTasks.Count);
		// but does not fail with an OptimisticLockingException
		assertNull(thread2.exception);
	  }
	}

}