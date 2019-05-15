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
namespace org.camunda.bpm.engine.test.api.mgmt.metrics
{

	using ProcessEngineImpl = org.camunda.bpm.engine.impl.ProcessEngineImpl;
	using CallerRunsRejectedJobsHandler = org.camunda.bpm.engine.impl.jobexecutor.CallerRunsRejectedJobsHandler;
	using DefaultJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.DefaultJobExecutor;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using Metrics = org.camunda.bpm.engine.management.Metrics;
	using ThreadControl = org.camunda.bpm.engine.test.concurrency.ConcurrencyTestCase.ThreadControl;
	using ControllableJobExecutor = org.camunda.bpm.engine.test.jobexecutor.ControllableJobExecutor;
	using Variables = org.camunda.bpm.engine.variable.Variables;


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobExecutorMetricsTest : AbstractMetricsTest
	{

	  protected internal JobExecutor jobExecutor;
	  protected internal ThreadPoolExecutor jobThreadPoolExecutor;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void setUp() throws Exception
	  protected internal override void setUp()
	  {
		base.setUp();
		jobExecutor = processEngineConfiguration.JobExecutor;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void tearDown() throws Exception
	  protected internal override void tearDown()
	  {
		base.tearDown();
		processEngineConfiguration.JobExecutor = jobExecutor;
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
	  public virtual void testJobAcquisitionMetricReporting()
	  {

		// given
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("asyncServiceTaskProcess");
		}

		// when
		waitForJobExecutorToProcessAllJobs(5000);
		processEngineConfiguration.DbMetricsReporter.reportNow();

		// then
		long acquisitionAttempts = managementService.createMetricsQuery().name(Metrics.JOB_ACQUISITION_ATTEMPT).sum();
		assertTrue(acquisitionAttempts >= 1);

		long acquiredJobs = managementService.createMetricsQuery().name(Metrics.JOB_ACQUIRED_SUCCESS).sum();
		assertEquals(3, acquiredJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
	  public virtual void testCompetingJobAcquisitionMetricReporting()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("asyncServiceTaskProcess");
		}

		// replace job executor
		ControllableJobExecutor jobExecutor1 = new ControllableJobExecutor((ProcessEngineImpl) processEngine);
		processEngineConfiguration.JobExecutor = jobExecutor1;
		ControllableJobExecutor jobExecutor2 = new ControllableJobExecutor((ProcessEngineImpl) processEngine);

		ThreadControl jobAcquisitionThread1 = jobExecutor1.AcquisitionThreadControl;
		ThreadControl jobAcquisitionThread2 = jobExecutor2.AcquisitionThreadControl;

		// when both executors are waiting to finish acquisition
		jobExecutor1.start();
		jobAcquisitionThread1.waitForSync(); // wait before starting acquisition
		jobAcquisitionThread1.makeContinueAndWaitForSync(); // wait before finishing acquisition

		jobExecutor2.start();
		jobAcquisitionThread2.waitForSync(); // wait before starting acquisition
		jobAcquisitionThread2.makeContinueAndWaitForSync(); // wait before finishing acquisition

		// thread 1 is able to acquire all jobs
		jobAcquisitionThread1.makeContinueAndWaitForSync();
		// thread 2 cannot acquire any jobs since they have been locked (and executed) by thread1 meanwhile
		jobAcquisitionThread2.makeContinueAndWaitForSync();

		processEngineConfiguration.DbMetricsReporter.reportNow();

		// then
		long acquisitionAttempts = managementService.createMetricsQuery().name(Metrics.JOB_ACQUISITION_ATTEMPT).sum();
		// each job executor twice (since the controllable thread always waits when already acquiring jobs)
		assertEquals(2 + 2, acquisitionAttempts);

		long acquiredJobs = managementService.createMetricsQuery().name(Metrics.JOB_ACQUIRED_SUCCESS).sum();
		assertEquals(3, acquiredJobs);

		long acquiredJobsFailure = managementService.createMetricsQuery().name(Metrics.JOB_ACQUIRED_FAILURE).sum();
		assertEquals(3, acquiredJobsFailure);

		// cleanup
		jobExecutor1.shutdown();
		jobExecutor2.shutdown();

		processEngineConfiguration.DbMetricsReporter.reportNow();
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
	  public virtual void testJobExecutionMetricReporting()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("asyncServiceTaskProcess");
		}
		for (int i = 0; i < 2; i++)
		{
		  runtimeService.startProcessInstanceByKey("asyncServiceTaskProcess", Variables.createVariables().putValue("fail", true));
		}

		// when
		waitForJobExecutorToProcessAllJobs(5000);

		// then
		long jobsSuccessful = managementService.createMetricsQuery().name(Metrics.JOB_SUCCESSFUL).sum();
		assertEquals(3, jobsSuccessful);

		long jobsFailed = managementService.createMetricsQuery().name(Metrics.JOB_FAILED).sum();
		// 2 jobs * 3 tries
		assertEquals(6, jobsFailed);

		long jobCandidatesForAcquisition = managementService.createMetricsQuery().name(Metrics.JOB_ACQUIRED_SUCCESS).sum();
		assertEquals(3 + 6, jobCandidatesForAcquisition);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Deployment public void testJobExecutionMetricExclusiveFollowUp()
	  public virtual void testJobExecutionMetricExclusiveFollowUp()
	  {
		// given
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("exclusiveServiceTasksProcess");
		}

		// when
		waitForJobExecutorToProcessAllJobs(5000);

		// then
		long jobsSuccessful = managementService.createMetricsQuery().name(Metrics.JOB_SUCCESSFUL).sum();
		assertEquals(6, jobsSuccessful);

		long jobsFailed = managementService.createMetricsQuery().name(Metrics.JOB_FAILED).sum();
		assertEquals(0, jobsFailed);

		// the respective follow-up jobs are exclusive and have been executed right away without
		// acquisition
		long jobCandidatesForAcquisition = managementService.createMetricsQuery().name(Metrics.JOB_ACQUIRED_SUCCESS).sum();
		assertEquals(3, jobCandidatesForAcquisition);

		long exclusiveFollowupJobs = managementService.createMetricsQuery().name(Metrics.JOB_LOCKED_EXCLUSIVE).sum();
		assertEquals(3, exclusiveFollowupJobs);
	  }

	  [Deployment(resources : "org/camunda/bpm/engine/test/api/mgmt/metrics/asyncServiceTaskProcess.bpmn20.xml")]
	  public virtual void testJobRejectedExecutionMetricReporting()
	  {
		// replace job executor with one that rejects all jobs
		RejectingJobExecutor rejectingExecutor = new RejectingJobExecutor();
		processEngineConfiguration.JobExecutor = rejectingExecutor;
		rejectingExecutor.registerProcessEngine((ProcessEngineImpl) processEngine);

		// given three jobs
		for (int i = 0; i < 3; i++)
		{
		  runtimeService.startProcessInstanceByKey("asyncServiceTaskProcess");
		}

		// when executing the jobs
		waitForJobExecutorToProcessAllJobs(5000L);

		// then all of them were rejected by the job executor which is reflected by the metric
		long numRejectedJobs = managementService.createMetricsQuery().name(Metrics.JOB_EXECUTION_REJECTED).sum();

		assertEquals(3, numRejectedJobs);
	  }

	  public class RejectingJobExecutor : DefaultJobExecutor
	  {

		public RejectingJobExecutor()
		{
		  BlockingQueue<ThreadStart> threadPoolQueue = new ArrayBlockingQueue<ThreadStart>(queueSize);
		  threadPoolExecutor = new ThreadPoolExecutorAnonymousInnerClass(this, corePoolSize, maxPoolSize, TimeUnit.MILLISECONDS, threadPoolQueue);
		  threadPoolExecutor.RejectedExecutionHandler = new ThreadPoolExecutor.AbortPolicy();

		  rejectedJobsHandler = new CallerRunsRejectedJobsHandler();
		}

		private class ThreadPoolExecutorAnonymousInnerClass : ThreadPoolExecutor
		{
			private readonly RejectingJobExecutor outerInstance;

			public ThreadPoolExecutorAnonymousInnerClass(RejectingJobExecutor outerInstance, int corePoolSize, int maxPoolSize, UnknownType MILLISECONDS, BlockingQueue<ThreadStart> threadPoolQueue) : base(corePoolSize, maxPoolSize, 0L, MILLISECONDS, threadPoolQueue)
			{
				this.outerInstance = outerInstance;
			}


			public override void execute(ThreadStart command)
			{
			  throw new RejectedExecutionException();
			}
		}
	  }

	}

}