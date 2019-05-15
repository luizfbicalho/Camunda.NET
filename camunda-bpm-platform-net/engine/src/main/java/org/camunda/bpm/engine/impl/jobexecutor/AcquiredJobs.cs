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


	/// <summary>
	/// @author Tom Baeyens
	/// @author Daniel Meyer
	/// </summary>
	public class AcquiredJobs
	{

	  protected internal int numberOfJobsAttemptedToAcquire;

	  protected internal IList<IList<string>> acquiredJobBatches = new List<IList<string>>();
	  protected internal ISet<string> acquiredJobs = new HashSet<string>();

	  protected internal int numberOfJobsFailedToLock = 0;

	  public AcquiredJobs(int numberOfJobsAttemptedToAcquire)
	  {
		this.numberOfJobsAttemptedToAcquire = numberOfJobsAttemptedToAcquire;
	  }

	  public virtual IList<IList<string>> JobIdBatches
	  {
		  get
		  {
			return acquiredJobBatches;
		  }
	  }

	  public virtual void addJobIdBatch(IList<string> jobIds)
	  {
		if (jobIds.Count > 0)
		{
		  acquiredJobBatches.Add(jobIds);
		  acquiredJobs.addAll(jobIds);
		}
	  }

	  public virtual void addJobIdBatch(string jobId)
	  {
		List<string> list = new List<string>();
		list.Add(jobId);

		addJobIdBatch(list);
	  }

	  public virtual bool contains(string jobId)
	  {
		return acquiredJobs.Contains(jobId);
	  }

	  public virtual int size()
	  {
		return acquiredJobs.Count;
	  }

	  public virtual void removeJobId(string id)
	  {
		numberOfJobsFailedToLock++;

		acquiredJobs.remove(id);

		IEnumerator<IList<string>> batchIterator = acquiredJobBatches.GetEnumerator();
		while (batchIterator.MoveNext())
		{
		  IList<string> batch = batchIterator.Current;
		  batch.Remove(id);

		  // remove batch if it is now empty
		  if (batch.Count == 0)
		  {
//JAVA TO C# CONVERTER TODO TASK: .NET enumerators are read-only:
			batchIterator.remove();
		  }

		}
	  }

	  public virtual int NumberOfJobsFailedToLock
	  {
		  get
		  {
			return numberOfJobsFailedToLock;
		  }
	  }

	  public virtual int NumberOfJobsAttemptedToAcquire
	  {
		  get
		  {
			return numberOfJobsAttemptedToAcquire;
		  }
	  }

	}

}