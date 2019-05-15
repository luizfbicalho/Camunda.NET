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
	using Page = org.camunda.bpm.engine.impl.Page;
	using AcquireJobsCmd = org.camunda.bpm.engine.impl.cmd.AcquireJobsCmd;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using AcquiredJobs = org.camunda.bpm.engine.impl.jobexecutor.AcquiredJobs;
	using JobExecutor = org.camunda.bpm.engine.impl.jobexecutor.JobExecutor;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using Before = org.junit.Before;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Matchers.any;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	public class AcquireJobCmdUnitTest
	{

	  protected internal const string PROCESS_INSTANCE_ID_1 = "pi_1";
	  protected internal const string PROCESS_INSTANCE_ID_2 = "pi_2";

	  protected internal const string JOB_ID_1 = "job_1";
	  protected internal const string JOB_ID_2 = "job_2";

	  protected internal AcquireJobsCmd acquireJobsCmd;
	  protected internal JobManager jobManager;
	  protected internal CommandContext commandContext;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void initCommand()
	  public virtual void initCommand()
	  {
		JobExecutor jobExecutor = mock(typeof(JobExecutor));
		when(jobExecutor.MaxJobsPerAcquisition).thenReturn(3);
		when(jobExecutor.LockOwner).thenReturn("test");
		when(jobExecutor.LockTimeInMillis).thenReturn(5 * 60 * 1000);

		acquireJobsCmd = new AcquireJobsCmd(jobExecutor);

		commandContext = mock(typeof(CommandContext));

		DbEntityManager dbEntityManager = mock(typeof(DbEntityManager));
		when(commandContext.DbEntityManager).thenReturn(dbEntityManager);

		jobManager = mock(typeof(JobManager));
		when(commandContext.JobManager).thenReturn(jobManager);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nonExclusiveJobsSameInstance()
	  public virtual void nonExclusiveJobsSameInstance()
	  {
		// given: two non-exclusive jobs for a different process instance
		JobEntity job1 = createNonExclusiveJob(JOB_ID_1, PROCESS_INSTANCE_ID_1);
		JobEntity job2 = createNonExclusiveJob(JOB_ID_2, PROCESS_INSTANCE_ID_1);

		// when the job executor acquire new jobs
		when(jobManager.findNextJobsToExecute(any(typeof(Page)))).thenReturn(Arrays.asList(job1, job2));

		// then the job executor should acquire job1 and job 2 in different batches
		checkThatAcquiredJobsInDifferentBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void nonExclusiveDifferentInstance()
	  public virtual void nonExclusiveDifferentInstance()
	  {
		// given: two non-exclusive jobs for the same process instance
		JobEntity job1 = createNonExclusiveJob(JOB_ID_1, PROCESS_INSTANCE_ID_1);
		JobEntity job2 = createNonExclusiveJob(JOB_ID_2, PROCESS_INSTANCE_ID_2);

		// when the job executor acquire new jobs
		when(jobManager.findNextJobsToExecute(any(typeof(Page)))).thenReturn(Arrays.asList(job1, job2));

		// then the job executor should acquire job1 and job 2 in different batches
		checkThatAcquiredJobsInDifferentBatches();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void exclusiveJobsSameInstance()
	  public virtual void exclusiveJobsSameInstance()
	  {
		// given: two exclusive jobs for the same process instance
		JobEntity job1 = createExclusiveJob(JOB_ID_1, PROCESS_INSTANCE_ID_1);
		JobEntity job2 = createExclusiveJob(JOB_ID_2, PROCESS_INSTANCE_ID_1);

		// when the job executor acquire new jobs
		when(jobManager.findNextJobsToExecute(any(typeof(Page)))).thenReturn(Arrays.asList(job1, job2));

		// then the job executor should acquire job1 and job 2 in one batch
		AcquiredJobs acquiredJobs = acquireJobsCmd.execute(commandContext);

		IList<IList<string>> jobIdBatches = acquiredJobs.JobIdBatches;
		assertThat(jobIdBatches.Count, @is(1));
		assertThat(jobIdBatches[0].Count, @is(2));
		assertThat(jobIdBatches[0], hasItems(JOB_ID_1, JOB_ID_2));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void exclusiveJobsDifferentInstance()
	  public virtual void exclusiveJobsDifferentInstance()
	  {
		// given: two exclusive jobs for a different process instance
		JobEntity job1 = createExclusiveJob(JOB_ID_1, PROCESS_INSTANCE_ID_1);
		JobEntity job2 = createExclusiveJob(JOB_ID_2, PROCESS_INSTANCE_ID_2);

		// when the job executor acquire new jobs
		when(jobManager.findNextJobsToExecute(any(typeof(Page)))).thenReturn(Arrays.asList(job1, job2));

		// then the job executor should acquire job1 and job 2 in different batches
		checkThatAcquiredJobsInDifferentBatches();
	  }

	  protected internal virtual JobEntity createExclusiveJob(string id, string processInstanceId)
	  {
		JobEntity job = createNonExclusiveJob(id, processInstanceId);
		when(job.Exclusive).thenReturn(true);
		return job;
	  }

	  protected internal virtual JobEntity createNonExclusiveJob(string id, string processInstanceId)
	  {
		JobEntity job = mock(typeof(JobEntity));
		when(job.Id).thenReturn(id);
		when(job.ProcessInstanceId).thenReturn(processInstanceId);
		return job;
	  }

	  protected internal virtual void checkThatAcquiredJobsInDifferentBatches()
	  {
		AcquiredJobs acquiredJobs = acquireJobsCmd.execute(commandContext);

		IList<IList<string>> jobIdBatches = acquiredJobs.JobIdBatches;
		assertThat(jobIdBatches.Count, @is(2));
		assertThat(jobIdBatches[0].Count, @is(1));
		assertThat(jobIdBatches[1].Count, @is(1));
	  }

	}

}