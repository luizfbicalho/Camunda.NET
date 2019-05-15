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
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobAcquisitionStrategy = org.camunda.bpm.engine.impl.jobexecutor.JobAcquisitionStrategy;
	using JobAcquisitionContext = org.camunda.bpm.engine.impl.jobexecutor.JobAcquisitionContext;
	using SequentialJobAcquisitionRunnable = org.camunda.bpm.engine.impl.jobexecutor.SequentialJobAcquisitionRunnable;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class RecordingAcquireJobsRunnable : SequentialJobAcquisitionRunnable
	{

	  protected internal IList<RecordedWaitEvent> waitEvents = new List<RecordedWaitEvent>();
	  protected internal IList<RecordedAcquisitionEvent> acquisitionEvents = new List<RecordedAcquisitionEvent>();

	  public RecordingAcquireJobsRunnable(ControllableJobExecutor jobExecutor) : base(jobExecutor)
	  {
	  }

	  protected internal override void suspendAcquisition(long millis)
	  {
		LOG.debugJobAcquisitionThreadSleeping(millis);
		if (jobExecutor is ControllableJobExecutor)
		{
		  ControllableJobExecutor controllableExecutor = (ControllableJobExecutor) jobExecutor;
		  if (controllableExecutor.SyncAsSuspendEnabled)
		  {
			controllableExecutor.AcquisitionThreadControl.sync();
		  }
		}
	  }

	  protected internal override AcquiredJobs acquireJobs(JobAcquisitionContext context, JobAcquisitionStrategy configuration, ProcessEngineImpl currentProcessEngine)
	  {
		acquisitionEvents.Add(new RecordedAcquisitionEvent(DateTimeHelper.CurrentUnixTimeMillis(), configuration.getNumJobsToAcquire(currentProcessEngine.Name)));
		return base.acquireJobs(context, configuration, currentProcessEngine);
	  }

	  public virtual IList<RecordedWaitEvent> WaitEvents
	  {
		  get
		  {
			return waitEvents;
		  }
	  }

	  public virtual IList<RecordedAcquisitionEvent> AcquisitionEvents
	  {
		  get
		  {
			return acquisitionEvents;
		  }
	  }

	  protected internal override void configureNextAcquisitionCycle(JobAcquisitionContext acquisitionContext, JobAcquisitionStrategy acquisitionStrategy)
	  {
		base.configureNextAcquisitionCycle(acquisitionContext, acquisitionStrategy);

		long timeBetweenCurrentAndNextAcquisition = acquisitionStrategy.WaitTime;
		waitEvents.Add(new RecordedWaitEvent(DateTimeHelper.CurrentUnixTimeMillis(), timeBetweenCurrentAndNextAcquisition));
	  }

	  public class RecordedWaitEvent
	  {

		protected internal long timestamp;
		protected internal long timeBetweenAcquisitions;

		public RecordedWaitEvent(long timestamp, long timeBetweenAcquisitions)
		{
		  this.timestamp = timestamp;
		  this.timeBetweenAcquisitions = timeBetweenAcquisitions;
		}

		public virtual long Timestamp
		{
			get
			{
			  return timestamp;
			}
		}
		public virtual long TimeBetweenAcquisitions
		{
			get
			{
			  return timeBetweenAcquisitions;
			}
		}
	  }

	  public class RecordedAcquisitionEvent
	  {
		protected internal long timestamp;
		protected internal int numJobsToAcquire;

		public RecordedAcquisitionEvent(long timestamp, int numJobsToAcquire)
		{
		  this.timestamp = timestamp;
		  this.numJobsToAcquire = numJobsToAcquire;
		}

		public virtual long Timestamp
		{
			get
			{
			  return timestamp;
			}
		}

		public virtual int NumJobsToAcquire
		{
			get
			{
			  return numJobsToAcquire;
			}
		}
	  }

	}

}