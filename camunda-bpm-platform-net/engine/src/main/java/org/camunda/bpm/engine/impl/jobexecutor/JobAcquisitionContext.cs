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
namespace org.camunda.bpm.engine.impl.jobexecutor
{

	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class JobAcquisitionContext
	{

	  protected internal IDictionary<string, IList<IList<string>>> rejectedJobBatchesByEngine;
	  protected internal IDictionary<string, AcquiredJobs> acquiredJobsByEngine;
	  protected internal IDictionary<string, IList<IList<string>>> additionalJobBatchesByEngine;
	  protected internal Exception acquisitionException;
	  protected internal long acquisitionTime;
	  protected internal bool isJobAdded;

	  public JobAcquisitionContext()
	  {
		this.rejectedJobBatchesByEngine = new Dictionary<string, IList<IList<string>>>();
		this.additionalJobBatchesByEngine = new Dictionary<string, IList<IList<string>>>();
		this.acquiredJobsByEngine = new Dictionary<string, AcquiredJobs>();
	  }

	  public virtual void submitRejectedBatch(string engineName, IList<string> jobIds)
	  {
		CollectionUtil.addToMapOfLists(rejectedJobBatchesByEngine, engineName, jobIds);
	  }

	  public virtual void submitAcquiredJobs(string engineName, AcquiredJobs acquiredJobs)
	  {
		acquiredJobsByEngine[engineName] = acquiredJobs;
	  }

	  public virtual void submitAdditionalJobBatch(string engineName, IList<string> jobIds)
	  {
		CollectionUtil.addToMapOfLists(additionalJobBatchesByEngine, engineName, jobIds);
	  }

	  public virtual void reset()
	  {
		additionalJobBatchesByEngine.Clear();

		// jobs that were rejected in the previous acquisition cycle
		// are to be resubmitted for execution in the current cycle
//JAVA TO C# CONVERTER TODO TASK: There is no .NET Dictionary equivalent to the Java 'putAll' method:
		additionalJobBatchesByEngine.putAll(rejectedJobBatchesByEngine);

		rejectedJobBatchesByEngine.Clear();
		acquiredJobsByEngine.Clear();
		acquisitionException = null;
		acquisitionTime = 0;
		isJobAdded = false;
	  }

	  /// <returns> true, if for all engines there were less jobs acquired than requested </returns>
	  public virtual bool areAllEnginesIdle()
	  {
		foreach (AcquiredJobs acquiredJobs in acquiredJobsByEngine.Values)
		{
		  int jobsAcquired = acquiredJobs.JobIdBatches.Count + acquiredJobs.NumberOfJobsFailedToLock;

		  if (jobsAcquired >= acquiredJobs.NumberOfJobsAttemptedToAcquire)
		  {
			return false;
		  }
		}

		return true;
	  }

	  /// <summary>
	  /// true if at least one job could not be locked, regardless of engine
	  /// </summary>
	  public virtual bool hasJobAcquisitionLockFailureOccurred()
	  {
		foreach (AcquiredJobs acquiredJobs in acquiredJobsByEngine.Values)
		{
		  if (acquiredJobs.NumberOfJobsFailedToLock > 0)
		  {
			return true;
		  }
		}

		return false;
	  }

	  // getters and setters

	  public virtual long AcquisitionTime
	  {
		  set
		  {
			this.acquisitionTime = value;
		  }
		  get
		  {
			return acquisitionTime;
		  }
	  }


	  /// <summary>
	  /// Jobs that were acquired in the current acquisition cycle.
	  /// @return
	  /// </summary>
	  public virtual IDictionary<string, AcquiredJobs> AcquiredJobsByEngine
	  {
		  get
		  {
			return acquiredJobsByEngine;
		  }
	  }

	  /// <summary>
	  /// Jobs that were rejected from execution in the acquisition cycle
	  /// due to lacking execution resources.
	  /// With an execution thread pool, these jobs could not be submitted due to
	  /// saturation of the underlying job queue.
	  /// </summary>
	  public virtual IDictionary<string, IList<IList<string>>> RejectedJobsByEngine
	  {
		  get
		  {
			return rejectedJobBatchesByEngine;
		  }
	  }

	  /// <summary>
	  /// Jobs that have been acquired in previous cycles and are supposed to
	  /// be re-submitted for execution
	  /// </summary>
	  public virtual IDictionary<string, IList<IList<string>>> AdditionalJobsByEngine
	  {
		  get
		  {
			return additionalJobBatchesByEngine;
		  }
	  }

	  public virtual Exception AcquisitionException
	  {
		  set
		  {
			this.acquisitionException = value;
		  }
		  get
		  {
			return acquisitionException;
		  }
	  }


	  public virtual bool JobAdded
	  {
		  set
		  {
			this.isJobAdded = value;
		  }
		  get
		  {
			return isJobAdded;
		  }
	  }

	}

}