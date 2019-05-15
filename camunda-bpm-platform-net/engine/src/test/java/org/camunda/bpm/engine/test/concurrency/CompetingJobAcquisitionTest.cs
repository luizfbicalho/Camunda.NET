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
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using PluggableProcessEngineTestCase = org.camunda.bpm.engine.impl.test.PluggableProcessEngineTestCase;
	using Logger = org.slf4j.Logger;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class CompetingJobAcquisitionTest : PluggableProcessEngineTestCase
	{

	private static Logger LOG = ProcessEngineLogger.TEST_LOGGER.Logger;

	  internal Thread testThread = Thread.CurrentThread;
	  internal static ControllableThread activeThread;
	  internal static string jobId;

	  public class JobAcquisitionThread : ControllableThread
	  {
		  private readonly CompetingJobAcquisitionTest outerInstance;

		  public JobAcquisitionThread(CompetingJobAcquisitionTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		internal OptimisticLockingException exception;
		internal AcquiredJobs jobs;

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
			JobExecutor jobExecutor = outerInstance.processEngineConfiguration.JobExecutor;
			jobs = (AcquiredJobs) outerInstance.processEngineConfiguration.CommandExecutorTxRequired.execute(new ControlledCommand(activeThread, new AcquireJobsCmd(jobExecutor)));

		  }
		  catch (OptimisticLockingException e)
		  {
			this.exception = e;
		  }
		  LOG.debug(Name + " ends");
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testCompetingJobAcquisitions() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void testCompetingJobAcquisitions()
	  {
		runtimeService.startProcessInstanceByKey("CompetingJobAcquisitionProcess");

		LOG.debug("test thread starts thread one");
		JobAcquisitionThread threadOne = new JobAcquisitionThread(this);
		threadOne.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread continues to start thread two");
		JobAcquisitionThread threadTwo = new JobAcquisitionThread(this);
		threadTwo.startAndWaitUntilControlIsReturned();

		LOG.debug("test thread notifies thread 1");
		threadOne.proceedAndWaitTillDone();
		assertNull(threadOne.exception);
		// the job was acquired
		assertEquals(1, threadOne.jobs.size());

		LOG.debug("test thread notifies thread 2");
		threadTwo.proceedAndWaitTillDone();
		// the acquisition did NOT fail
		assertNull(threadTwo.exception);
		// but the job was not acquired
		assertEquals(0, threadTwo.jobs.size());

	  }

	}

}