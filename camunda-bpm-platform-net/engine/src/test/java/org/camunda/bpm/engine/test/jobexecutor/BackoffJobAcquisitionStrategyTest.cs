using System;

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

	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using BackoffJobAcquisitionStrategy = org.camunda.bpm.engine.impl.jobexecutor.BackoffJobAcquisitionStrategy;
	using JobAcquisitionContext = org.camunda.bpm.engine.impl.jobexecutor.JobAcquisitionContext;
	using JobAcquisitionStrategy = org.camunda.bpm.engine.impl.jobexecutor.JobAcquisitionStrategy;
	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class BackoffJobAcquisitionStrategyTest
	{

	  // strategy configuration
	  protected internal const long BASE_IDLE_WAIT_TIME = 50;
	  protected internal const float IDLE_INCREASE_FACTOR = 1.5f;
	  protected internal const long MAX_IDLE_TIME = 500;

	  protected internal const long BASE_BACKOFF_WAIT_TIME = 80;
	  protected internal const float BACKOFF_INCREASE_FACTOR = 2.0f;
	  protected internal const long MAX_BACKOFF_TIME = 1000;

	  protected internal const int DECREASE_THRESHOLD = 3;
	  protected internal const int NUM_JOBS_TO_ACQUIRE = 10;

	  // misc
	  protected internal const string ENGINE_NAME = "engine";

	  protected internal JobAcquisitionStrategy strategy;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		strategy = new BackoffJobAcquisitionStrategy(BASE_IDLE_WAIT_TIME, IDLE_INCREASE_FACTOR, MAX_IDLE_TIME, BASE_BACKOFF_WAIT_TIME, BACKOFF_INCREASE_FACTOR, MAX_BACKOFF_TIME, DECREASE_THRESHOLD, NUM_JOBS_TO_ACQUIRE);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIdleWaitTime()
	  public virtual void testIdleWaitTime()
	  {

		// given a job acquisition strategy and a job acquisition context
		// with no acquired jobs
		JobAcquisitionContext context = new JobAcquisitionContext();

		context.submitAcquiredJobs(ENGINE_NAME, buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, 0, 0));

		// when reconfiguring the strategy
		strategy.reconfigure(context);

		// then the job acquisition strategy returns the level 1 idle time
		Assert.assertEquals(BASE_IDLE_WAIT_TIME, strategy.WaitTime);

		// when resubmitting the same acquisition result
		for (int idleLevel = 1; idleLevel < 6; idleLevel++)
		{
		  context.reset();
		  context.submitAcquiredJobs(ENGINE_NAME, buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, 0, 0));

		  strategy.reconfigure(context);
		  Assert.assertEquals((long)(BASE_IDLE_WAIT_TIME * Math.Pow(IDLE_INCREASE_FACTOR, idleLevel)), strategy.WaitTime);
		}

		// and the maximum idle level is finally reached
		context.reset();
		context.submitAcquiredJobs(ENGINE_NAME, buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, 0, 0));

		strategy.reconfigure(context);
		Assert.assertEquals(MAX_IDLE_TIME, strategy.WaitTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAcquisitionAfterIdleWait()
	  public virtual void testAcquisitionAfterIdleWait()
	  {

		// given a job acquisition strategy and a job acquisition context
		// with no acquired jobs
		JobAcquisitionContext context = new JobAcquisitionContext();

		context.submitAcquiredJobs(ENGINE_NAME, buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, 0, 0));
		strategy.reconfigure(context);
		Assert.assertEquals(BASE_IDLE_WAIT_TIME, strategy.WaitTime);

		// when receiving a successful acquisition result
		context.reset();
		context.submitAcquiredJobs(ENGINE_NAME, buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, NUM_JOBS_TO_ACQUIRE, 0));

		strategy.reconfigure(context);

		// then the idle wait time has been reset
		Assert.assertEquals(0L, strategy.WaitTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testAcquireLessJobsOnRejection()
	  public virtual void testAcquireLessJobsOnRejection()
	  {
		// given a job acquisition strategy and a job acquisition context
		// with acquired jobs, some of which have been rejected for execution
		JobAcquisitionContext context = new JobAcquisitionContext();

		AcquiredJobs acquiredJobs = buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, NUM_JOBS_TO_ACQUIRE, 0);
		context.submitAcquiredJobs(ENGINE_NAME, acquiredJobs);

		// when half of the jobs are rejected
		int numJobsRejected = 5;
		for (int i = 0; i < numJobsRejected; i++)
		{
		  context.submitRejectedBatch(ENGINE_NAME, acquiredJobs.JobIdBatches[i]);
		}

		// then the strategy only attempts to acquire the number of jobs that were successfully submitted
		strategy.reconfigure(context);

		Assert.assertEquals(NUM_JOBS_TO_ACQUIRE - numJobsRejected, strategy.getNumJobsToAcquire(ENGINE_NAME));

		// without a timeout
		Assert.assertEquals(0, strategy.WaitTime);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWaitTimeOnFullRejection()
	  public virtual void testWaitTimeOnFullRejection()
	  {
		// given a job acquisition strategy and a job acquisition context
		// with acquired jobs all of which have been rejected for execution
		JobAcquisitionContext context = new JobAcquisitionContext();

		AcquiredJobs acquiredJobs = buildAcquiredJobs(NUM_JOBS_TO_ACQUIRE, NUM_JOBS_TO_ACQUIRE, 0);
		context.submitAcquiredJobs(ENGINE_NAME, acquiredJobs);

		for (int i = 0; i < NUM_JOBS_TO_ACQUIRE; i++)
		{
		  context.submitRejectedBatch(ENGINE_NAME, acquiredJobs.JobIdBatches[i]);
		}

		// when reconfiguring the strategy
		strategy.reconfigure(context);

		// then there is a slight wait time to avoid constant spinning while
		// no execution resources are available
		Assert.assertEquals(BackoffJobAcquisitionStrategy.DEFAULT_EXECUTION_SATURATION_WAIT_TIME, strategy.WaitTime);
	  }

	  /// <summary>
	  /// numJobsToAcquire >= numJobsAcquired >= numJobsFailedToLock must hold
	  /// </summary>
	  protected internal virtual AcquiredJobs buildAcquiredJobs(int numJobsToAcquire, int numJobsAcquired, int numJobsFailedToLock)
	  {
		AcquiredJobs acquiredJobs = new AcquiredJobs(numJobsToAcquire);
		for (int i = 0; i < numJobsAcquired; i++)
		{
		  acquiredJobs.addJobIdBatch(Arrays.asList(Convert.ToString(i)));
		}

		for (int i = 0; i < numJobsFailedToLock; i++)
		{
		  acquiredJobs.removeJobId(Convert.ToString(i));
		}

		return acquiredJobs;
	  }

	}

}