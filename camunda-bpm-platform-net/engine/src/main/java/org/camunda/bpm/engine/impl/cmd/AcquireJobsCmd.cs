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
namespace org.camunda.bpm.engine.impl.cmd
{
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using OptimisticLockingListener = org.camunda.bpm.engine.impl.db.entitymanager.OptimisticLockingListener;
	using DbEntityOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbEntityOperation;
	using DbOperation = org.camunda.bpm.engine.impl.db.entitymanager.operation.DbOperation;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;


	/// <summary>
	/// @author Nick Burch
	/// @author Daniel Meyer
	/// </summary>
	public class AcquireJobsCmd : Command<AcquiredJobs>, OptimisticLockingListener
	{

	  private readonly JobExecutor jobExecutor;

	  protected internal AcquiredJobs acquiredJobs;
	  protected internal int numJobsToAcquire;

	  public AcquireJobsCmd(JobExecutor jobExecutor) : this(jobExecutor, jobExecutor.MaxJobsPerAcquisition)
	  {
	  }

	  public AcquireJobsCmd(JobExecutor jobExecutor, int numJobsToAcquire)
	  {
		this.jobExecutor = jobExecutor;
		this.numJobsToAcquire = numJobsToAcquire;
	  }

	  public virtual AcquiredJobs execute(CommandContext commandContext)
	  {

		acquiredJobs = new AcquiredJobs(numJobsToAcquire);

		IList<JobEntity> jobs = commandContext.JobManager.findNextJobsToExecute(new Page(0, numJobsToAcquire));

		IDictionary<string, IList<string>> exclusiveJobsByProcessInstance = new Dictionary<string, IList<string>>();

		foreach (JobEntity job in jobs)
		{

		  lockJob(job);

		  if (job.Exclusive)
		  {
			IList<string> list = exclusiveJobsByProcessInstance[job.ProcessInstanceId];
			if (list == null)
			{
			  list = new List<string>();
			  exclusiveJobsByProcessInstance[job.ProcessInstanceId] = list;
			}
			list.Add(job.Id);
		  }
		  else
		  {
			acquiredJobs.addJobIdBatch(job.Id);
		  }
		}

		foreach (IList<string> jobIds in exclusiveJobsByProcessInstance.Values)
		{
		  acquiredJobs.addJobIdBatch(jobIds);
		}

		// register an OptimisticLockingListener which is notified about jobs which cannot be acquired.
		// the listener removes them from the list of acquired jobs.
		commandContext.DbEntityManager.registerOptimisticLockingListener(this);


		return acquiredJobs;
	  }

	  protected internal virtual void lockJob(JobEntity job)
	  {
		string lockOwner = jobExecutor.LockOwner;
		job.LockOwner = lockOwner;

		int lockTimeInMillis = jobExecutor.LockTimeInMillis;

		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		gregorianCalendar.Time = ClockUtil.CurrentTime;
		gregorianCalendar.add(DateTime.MILLISECOND, lockTimeInMillis);
		job.LockExpirationTime = gregorianCalendar.Time;
	  }

	  public virtual Type EntityType
	  {
		  get
		  {
			return typeof(JobEntity);
		  }
	  }

	  public virtual void failedOperation(DbOperation operation)
	  {
		if (operation is DbEntityOperation)
		{

		  DbEntityOperation entityOperation = (DbEntityOperation) operation;
		  if (entityOperation.EntityType.IsAssignableFrom(typeof(JobEntity)))
		  {
			// could not lock the job -> remove it from list of acquired jobs
			acquiredJobs.removeJobId(entityOperation.Entity.Id);
		  }

		}
	  }

	}

}