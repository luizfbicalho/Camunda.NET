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


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using Job = org.camunda.bpm.engine.runtime.Job;
	using JobQuery = org.camunda.bpm.engine.runtime.JobQuery;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Task = org.camunda.bpm.engine.task.Task;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using RecordedWaitEvent = org.camunda.bpm.engine.test.jobexecutor.RecordingAcquireJobsRunnable.RecordedWaitEvent;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using After = org.junit.After;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static junit.framework.TestCase.assertNotNull;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobAcquisitionBackoffIdleTest
	{
		private bool InstanceFieldsInitialized = false;

		public JobAcquisitionBackoffIdleTest()
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


	  public const int BASE_IDLE_WAIT_TIME = 5000;
	  public const int MAX_IDLE_WAIT_TIME = 60000;

	  protected internal ControllableJobExecutor jobExecutor;
	  protected internal ThreadControl acquisitionThread;

	  protected internal ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRuleAnonymousInnerClass();

	  private class ProcessEngineBootstrapRuleAnonymousInnerClass : ProcessEngineBootstrapRule
	  {
		  public override ProcessEngineConfiguration configureEngine(ProcessEngineConfigurationImpl configuration)
		  {
			outerInstance.jobExecutor = new ControllableJobExecutor(true);
			outerInstance.jobExecutor.MaxJobsPerAcquisition = 1;
			outerInstance.jobExecutor.WaitTimeInMillis = BASE_IDLE_WAIT_TIME;
			outerInstance.jobExecutor.MaxWait = MAX_IDLE_WAIT_TIME;
			outerInstance.acquisitionThread = outerInstance.jobExecutor.AcquisitionThreadControl;

			return configuration.setJobExecutor(outerInstance.jobExecutor);
		  }
	  }
	  protected internal ProvidedProcessEngineRule engineRule = new ProvidedProcessEngineRule(bootstrapRule);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(bootstrapRule).around(engineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void shutdownJobExecutor()
	  public virtual void shutdownJobExecutor()
	  {
		jobExecutor.shutdown();
	  }

	  protected internal virtual void cycleJobAcquisitionToMaxIdleTime()
	  {
		// cycle of job acquisition
		// => 0 jobs are acquired
		// => acquisition should become idle
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(BASE_IDLE_WAIT_TIME);

		// another cycle of job acquisition
		// => 0 jobs are acquired
		// => acquisition should increase idle time
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(BASE_IDLE_WAIT_TIME * 2);

		// another cycle of job acquisition
		// => 0 jobs are acquired
		// => acquisition should increase idle time exponentially
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(BASE_IDLE_WAIT_TIME * 4);

		// another cycle of job acquisition
		// => 0 jobs are acquired
		// => acquisition should increase idle time exponentially
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(BASE_IDLE_WAIT_TIME * 8);

		// another cycle of job acquisition
		// => 0 jobs are acquired
		// => acquisition should increase to max idle time
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(MAX_IDLE_WAIT_TIME);
	  }

	  /// <summary>
	  /// CAM-5073
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml") public void testIdlingAfterConcurrentJobAddedNotification()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/simpleAsyncProcess.bpmn20.xml")]
	  public virtual void testIdlingAfterConcurrentJobAddedNotification()
	  {
		// start job acquisition - waiting before acquiring jobs
		jobExecutor.start();
		acquisitionThread.waitForSync();

		// acquire jobs
		acquisitionThread.makeContinueAndWaitForSync();

		// issue a message added notification
		engineRule.RuntimeService.startProcessInstanceByKey("simpleAsyncProcess");

		// complete job acquisition - trigger re-configuration
		// => due to the hint, the job executor should not become idle
		acquisitionThread.makeContinueAndWaitForSync();
		assertJobExecutorWaitEvent(0L);

		// another cycle of job acquisition
		// => acquires and executes the new job
		// => acquisition does not become idle because enough jobs could be acquired
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(0L);

		cycleJobAcquisitionToMaxIdleTime();
	  }

	  protected internal virtual void initAcquisitionAndIdleToMaxTime()
	  {
		// start job acquisition - waiting before acquiring jobs
		jobExecutor.start();
		acquisitionThread.waitForSync();

		//cycle acquistion till max idle time is reached
		cycleJobAcquisitionToMaxIdleTime();
	  }

	  protected internal virtual void cycleAcquisitionAndAssertAfterJobExecution(JobQuery jobQuery)
	  {
		// another cycle of job acquisition after acuqisition idle was reseted
		// => 1 jobs are acquired
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(0);

		// we have no timer to fire
		assertEquals(0, jobQuery.count());

		// and we are in the second state
		assertEquals(1L, engineRule.TaskService.createTaskQuery().count());
		Task task = engineRule.TaskService.createTaskQuery().orderByTaskName().desc().singleResult();
		assertEquals("Next Task", task.Name);
		// complete the task and end the execution
		engineRule.TaskService.complete(task.Id);
	  }

	  public interface JobCreationInCycle
	  {

		ProcessInstance createJobAndContinueCycle();
	  }

	  public virtual void testIdlingWithHint(JobCreationInCycle jobCreationInCycle)
	  {
		initAcquisitionAndIdleToMaxTime();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final java.util.Date startTime = new java.util.Date();
		DateTime startTime = DateTime.Now;
		ProcessInstance procInstance = jobCreationInCycle.createJobAndContinueCycle();

		 // After process start, there should be 1 timer created
		Task task1 = engineRule.TaskService.createTaskQuery().singleResult();
		assertEquals("Timer Task", task1.Name);
		//and one job
		JobQuery jobQuery = engineRule.ManagementService.createJobQuery().processInstanceId(procInstance.Id);
		Job job = jobQuery.singleResult();
		assertNotNull(job);

		// the hint of the added job resets the idle time
		// => 0 jobs are acquired so we had to wait BASE IDLE TIME
		//after this time we can acquire the timer
		triggerReconfigurationAndNextCycle();
		assertJobExecutorWaitEvent(BASE_IDLE_WAIT_TIME);

		//time is increased so timer is found
		ClockUtil.CurrentTime = new DateTime(startTime.Ticks + BASE_IDLE_WAIT_TIME);
		//now we are able to acquire the job
		cycleAcquisitionAndAssertAfterJobExecution(jobQuery);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml") public void testIdlingWithHintOnSuspend()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml")]
	  public virtual void testIdlingWithHintOnSuspend()
	  {
		testIdlingWithHint(new JobCreationInCycleAnonymousInnerClass(this));
	  }

	  private class JobCreationInCycleAnonymousInnerClass : JobCreationInCycle
	  {
		  private readonly JobAcquisitionBackoffIdleTest outerInstance;

		  public JobCreationInCycleAnonymousInnerClass(JobAcquisitionBackoffIdleTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessInstance createJobAndContinueCycle()
		  {
			//continue sync before acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//continue sync after acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();

			//process is started with timer boundary event which should start after 3 seconds
			ProcessInstance procInstance = outerInstance.engineRule.RuntimeService.startProcessInstanceByKey("timer-example");
			//release suspend sync
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//assert max idle time and clear events
			outerInstance.assertJobExecutorWaitEvent(MAX_IDLE_WAIT_TIME);

			//trigger continue and assert that new acquisition cycle was triggered right after the hint
			outerInstance.triggerReconfigurationAndNextCycle();
			outerInstance.assertJobExecutorWaitEvent(0);
			return procInstance;
		  }
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml") public void testIdlingWithHintOnAquisition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml")]
	  public virtual void testIdlingWithHintOnAquisition()
	  {
		testIdlingWithHint(new JobCreationInCycleAnonymousInnerClass2(this));
	  }

	  private class JobCreationInCycleAnonymousInnerClass2 : JobCreationInCycle
	  {
		  private readonly JobAcquisitionBackoffIdleTest outerInstance;

		  public JobCreationInCycleAnonymousInnerClass2(JobAcquisitionBackoffIdleTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessInstance createJobAndContinueCycle()
		  {
			//continue sync before acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();

			//process is started with timer boundary event which should start after 3 seconds
			ProcessInstance procInstance = outerInstance.engineRule.RuntimeService.startProcessInstanceByKey("timer-example");

			//continue sync after acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//release suspend sync
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//assert max idle time and clear events
			outerInstance.assertJobExecutorWaitEvent(0);
			return procInstance;
		  }
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @Deployment(resources = "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml") public void testIdlingWithHintBeforeAquisition()
	  [Deployment(resources : "org/camunda/bpm/engine/test/jobexecutor/JobAcquisitionBackoffIdleTest.testShortTimerOnUserTaskWithExpression.bpmn20.xml")]
	  public virtual void testIdlingWithHintBeforeAquisition()
	  {
		testIdlingWithHint(new JobCreationInCycleAnonymousInnerClass3(this));
	  }

	  private class JobCreationInCycleAnonymousInnerClass3 : JobCreationInCycle
	  {
		  private readonly JobAcquisitionBackoffIdleTest outerInstance;

		  public JobCreationInCycleAnonymousInnerClass3(JobAcquisitionBackoffIdleTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public ProcessInstance createJobAndContinueCycle()
		  {
			//process is started with timer boundary event which should start after 3 seconds
			ProcessInstance procInstance = outerInstance.engineRule.RuntimeService.startProcessInstanceByKey("timer-example");

			//continue sync before acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//continue sync after acquire
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//release suspend sync
			outerInstance.acquisitionThread.makeContinueAndWaitForSync();
			//assert max idle time and clear events
			outerInstance.assertJobExecutorWaitEvent(0);
			return procInstance;
		  }
	  }

	  protected internal virtual void triggerReconfigurationAndNextCycle()
	  {
		acquisitionThread.makeContinueAndWaitForSync();
		acquisitionThread.makeContinueAndWaitForSync();
		acquisitionThread.makeContinueAndWaitForSync();
	  }

	  protected internal virtual void assertJobExecutorWaitEvent(long expectedTimeout)
	  {
		IList<RecordedWaitEvent> waitEvents = jobExecutor.AcquireJobsRunnable.WaitEvents;
		assertEquals(1, waitEvents.Count);
		assertEquals(expectedTimeout, waitEvents[0].TimeBetweenAcquisitions);

		// discard wait event if successfully asserted
		waitEvents.Clear();
	  }

	}

}