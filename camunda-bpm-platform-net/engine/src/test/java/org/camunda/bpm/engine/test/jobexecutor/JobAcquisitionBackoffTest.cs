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

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using RecordedAcquisitionEvent = org.camunda.bpm.engine.test.jobexecutor.RecordingAcquireJobsRunnable.RecordedAcquisitionEvent;
	using RecordedWaitEvent = org.camunda.bpm.engine.test.jobexecutor.RecordingAcquireJobsRunnable.RecordedWaitEvent;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using After = org.junit.After;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobAcquisitionBackoffTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobAcquisitionBackoffTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(bootstrapRule).around(engineRule);
		}


	  protected internal const int BASE_BACKOFF_TIME = 1000;
	  protected internal const int MAX_BACKOFF_TIME = 5000;
	  protected internal const int BACKOFF_FACTOR = 2;
	  protected internal const int BACKOFF_DECREASE_THRESHOLD = 2;
	  protected internal const int DEFAULT_NUM_JOBS_TO_ACQUIRE = 3;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			return configuration.setJobExecutor(new ControllableJobExecutor());
		  }
	  }
	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

	  protected internal ControllableJobExecutor jobExecutor1;
	  protected internal ControllableJobExecutor jobExecutor2;

	  protected internal ThreadControl acquisitionThread1;
	  protected internal ThreadControl acquisitionThread2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void setUp()
	  {
		jobExecutor1 = (ControllableJobExecutor)((ProcessEngineConfigurationImpl) engineRule.ProcessEngine.ProcessEngineConfiguration).JobExecutor;
		jobExecutor1.MaxJobsPerAcquisition = DEFAULT_NUM_JOBS_TO_ACQUIRE;
		jobExecutor1.BackoffTimeInMillis = BASE_BACKOFF_TIME;
		jobExecutor1.MaxBackoff = MAX_BACKOFF_TIME;
		jobExecutor1.BackoffDecreaseThreshold = BACKOFF_DECREASE_THRESHOLD;
		acquisitionThread1 = jobExecutor1.AcquisitionThreadControl;

		jobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl) engineRule.ProcessEngine);
		jobExecutor2.MaxJobsPerAcquisition = DEFAULT_NUM_JOBS_TO_ACQUIRE;
		jobExecutor2.BackoffTimeInMillis = BASE_BACKOFF_TIME;
		jobExecutor2.MaxBackoff = MAX_BACKOFF_TIME;
		jobExecutor2.BackoffDecreaseThreshold = BACKOFF_DECREASE_THRESHOLD;
		acquisitionThread2 = jobExecutor2.AcquisitionThreadControl;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual void tearDown()
	  {
		jobExecutor1.shutdown();
		jobExecutor2.shutdown();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testBackoffOnOptimisticLocking()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testBackoffOnOptimisticLocking()
	  {
		// when starting a number of process instances process instance
		for (int i = 0; i < 9; i++)
		{
		  engineRule.RuntimeService.startProcessInstanceByKey("simpleAsyncProcess").Id;
		}

		// ensure that both acquisition threads acquire the same jobs thereby provoking an optimistic locking exception
		JobAcquisitionTestHelper.suspendInstances(engineRule.ProcessEngine, 6);

		// when starting job execution, both acquisition threads wait before acquiring something
		jobExecutor1.start();
		acquisitionThread1.waitForSync();
		jobExecutor2.start();
		acquisitionThread2.waitForSync();

		// when having both threads acquire jobs
		// then both wait before committing the acquiring transaction (AcquireJobsCmd)
		acquisitionThread1.makeContinueAndWaitForSync();
		acquisitionThread2.makeContinueAndWaitForSync();

		// when continuing acquisition thread 1
		acquisitionThread1.makeContinueAndWaitForSync();

		// then it has not performed waiting since it was able to acquire and execute all jobs
		IList<RecordedWaitEvent> jobExecutor1WaitEvents = jobExecutor1.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(1, jobExecutor1WaitEvents.Count);
		Assert.assertEquals(0, jobExecutor1WaitEvents[0].TimeBetweenAcquisitions);

		// when continuing acquisition thread 2, acquisition fails with an OLE
		acquisitionThread2.makeContinueAndWaitForSync();

		// and has performed backoff
		IList<RecordedWaitEvent> jobExecutor2WaitEvents = jobExecutor2.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(1, jobExecutor2WaitEvents.Count);
		RecordedWaitEvent waitEvent = jobExecutor2WaitEvents[0];
		// we don't know the exact wait time,
		// since there is random jitter applied
		JobAcquisitionTestHelper.assertInBetween(BASE_BACKOFF_TIME, BASE_BACKOFF_TIME + BASE_BACKOFF_TIME / 2, waitEvent.TimeBetweenAcquisitions);

		// when performing another cycle of acquisition
		JobAcquisitionTestHelper.activateInstances(engineRule.ProcessEngine, 6);
		acquisitionThread1.makeContinueAndWaitForSync();
		acquisitionThread2.makeContinueAndWaitForSync();

		// and thread 1 again acquires all jobs successfully
		acquisitionThread1.makeContinueAndWaitForSync();

		// while thread 2 again fails with OLE
		acquisitionThread2.makeContinueAndWaitForSync();

		// then thread 1 has tried to acquired 3 jobs again
		IList<RecordedAcquisitionEvent> jobExecutor1AcquisitionEvents = jobExecutor1.AcquireJobsRunnable.AcquisitionEvents;
		RecordedAcquisitionEvent secondAcquisitionAttempt = jobExecutor1AcquisitionEvents[1];
		Assert.assertEquals(3, secondAcquisitionAttempt.NumJobsToAcquire);

		// and not waited
		jobExecutor1WaitEvents = jobExecutor1.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(2, jobExecutor1WaitEvents.Count);
		Assert.assertEquals(0, jobExecutor1WaitEvents[1].TimeBetweenAcquisitions);

		// then thread 2 has tried to acquire 6 jobs this time
		IList<RecordedAcquisitionEvent> jobExecutor2AcquisitionEvents = jobExecutor2.AcquireJobsRunnable.AcquisitionEvents;
		secondAcquisitionAttempt = jobExecutor2AcquisitionEvents[1];
		Assert.assertEquals(6, secondAcquisitionAttempt.NumJobsToAcquire);

		// and again increased its backoff
		jobExecutor2WaitEvents = jobExecutor2.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(2, jobExecutor2WaitEvents.Count);
		RecordedWaitEvent secondWaitEvent = jobExecutor2WaitEvents[1];
		long expectedBackoffTime = BASE_BACKOFF_TIME * BACKOFF_FACTOR; // 1000 * 2^1
		JobAcquisitionTestHelper.assertInBetween(expectedBackoffTime, expectedBackoffTime + expectedBackoffTime / 2, secondWaitEvent.TimeBetweenAcquisitions);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testBackoffDecrease()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testBackoffDecrease()
	  {
		// when starting a number of process instances process instance
		for (int i = 0; i < 15; i++)
		{
		  engineRule.RuntimeService.startProcessInstanceByKey("simpleAsyncProcess").Id;
		}

		// ensure that both acquisition threads acquire the same jobs thereby provoking an optimistic locking exception
		JobAcquisitionTestHelper.suspendInstances(engineRule.ProcessEngine, 12);

		// when starting job execution, both acquisition threads wait before acquiring something
		jobExecutor1.start();
		acquisitionThread1.waitForSync();
		jobExecutor2.start();
		acquisitionThread2.waitForSync();

		// when continuing acquisition thread 1
		// then it is able to acquire and execute all jobs
		acquisitionThread1.makeContinueAndWaitForSync();

		// when continuing acquisition thread 2
		// acquisition fails with an OLE
		acquisitionThread2.makeContinueAndWaitForSync();

		jobExecutor1.shutdown();
		acquisitionThread1.waitUntilDone();
		acquisitionThread2.makeContinueAndWaitForSync();

		// such that acquisition thread 2 performs backoff
		IList<RecordedWaitEvent> jobExecutor2WaitEvents = jobExecutor2.AcquireJobsRunnable.WaitEvents;
		Assert.assertEquals(1, jobExecutor2WaitEvents.Count);

		// when in the next cycles acquisition thread2 successfully acquires jobs without OLE for n times
		JobAcquisitionTestHelper.activateInstances(engineRule.ProcessEngine, 12);

		for (int i = 0; i < BACKOFF_DECREASE_THRESHOLD; i++)
		{
		  // backoff has not decreased yet
		  Assert.assertTrue(jobExecutor2WaitEvents[i].TimeBetweenAcquisitions > 0);

		  acquisitionThread2.makeContinueAndWaitForSync(); // acquire
		  acquisitionThread2.makeContinueAndWaitForSync(); // continue after acquisition with next cycle
		}

		// it decreases its backoff again
		long lastBackoff = jobExecutor2WaitEvents[BACKOFF_DECREASE_THRESHOLD].TimeBetweenAcquisitions;
		Assert.assertEquals(0, lastBackoff);
	  }


	}

}