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
	using ActivityBehavior = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityBehavior;
	using ActivityExecution = org.camunda.bpm.engine.impl.pvm.@delegate.ActivityExecution;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CompetingSignalsTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;

	  public class SignalThread : ControllableThread
	  {
		  private readonly CompetingSignalsTest outerInstance;


		internal string executionId;
		internal OptimisticLockingException exception;

		public SignalThread(CompetingSignalsTest outerInstance, string executionId)
		{
			this.outerInstance = outerInstance;
		  this.executionId = executionId;
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
			outerInstance.runtimeService.signal(executionId);
		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

	  public class ControlledConcurrencyBehavior : ActivityBehavior
	  {
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.impl.pvm.delegate.ActivityExecution execution) throws Exception
		public virtual void execute(ActivityExecution execution)
		{
		  activeThread.returnControlToTestThreadAndWait();
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompetingSignals() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCompetingSignals()
	  {
		ProcessInstance processInstance = runtimeService.startProcessInstanceByKey("CompetingSignalsProcess");
		string processInstanceId = processInstance.Id;

		LOG.debug("test thread starts thread one");
		SignalThread threadOne = new SignalThread(this, processInstanceId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		SignalThread threadTwo = new SignalThread(this, processInstanceId);
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