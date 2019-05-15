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
	using ActivityInstanceCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityInstanceCancellationCmd;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using ActivityInstance = org.camunda.bpm.engine.runtime.ActivityInstance;
	using Logger = org.slf4j.Logger;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class CompetingActivityInstanceCancellationTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;
	  internal static string jobId;

	  public class CancelActivityInstance : ControllableThread
	  {
		  private readonly CompetingActivityInstanceCancellationTest outerInstance;


		internal string processInstanceId;
		internal string activityInstanceId;
		internal OptimisticLockingException exception;

		public CancelActivityInstance(CompetingActivityInstanceCancellationTest outerInstance, string processInstanceId, string activityInstanceId)
		{
			this.outerInstance = outerInstance;
		  this.processInstanceId = processInstanceId;
		  this.activityInstanceId = activityInstanceId;
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
			outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new ActivityInstanceCancellationCmd(processInstanceId, activityInstanceId)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Deployment(resources = {"org/camunda/bpm/engine/test/concurrency/CompetingForkTest.testCompetingFork.bpmn20.xml"}) public void testCompetingCancellation() throws Exception
	  [Deployment(resources : {"org/camunda/bpm/engine/test/concurrency/CompetingForkTest.testCompetingFork.bpmn20.xml"})]
	  public virtual void testCompetingCancellation()
	  {
		string processInstanceId = runtimeService.startProcessInstanceByKey("process").Id;

		ActivityInstance activityInstance = runtimeService.getActivityInstance(processInstanceId);
		ActivityInstance[] children = activityInstance.ChildActivityInstances;

		string task1ActivityInstanceId = null;
		string task2ActivityInstanceId = null;
		string task3ActivityInstanceId = null;

		foreach (ActivityInstance currentInstance in children)
		{

		  string id = currentInstance.Id;
		  string activityId = currentInstance.ActivityId;

		  if ("task1".Equals(activityId))
		  {
			task1ActivityInstanceId = id;
		  }
		  else if ("task2".Equals(activityId))
		  {
			task2ActivityInstanceId = id;
		  }
		  else if ("task3".Equals(activityId))
		  {
			task3ActivityInstanceId = id;
		  }
		  else
		  {
			fail();
		  }
		}

		LOG.debug("test thread starts thread one");
		CancelActivityInstance threadOne = new CancelActivityInstance(this, processInstanceId, task1ActivityInstanceId);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread thread two");
		CancelActivityInstance threadTwo = new CancelActivityInstance(this, processInstanceId, task2ActivityInstanceId);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread three");
		CancelActivityInstance threadThree = new CancelActivityInstance(this, processInstanceId, task3ActivityInstanceId);
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