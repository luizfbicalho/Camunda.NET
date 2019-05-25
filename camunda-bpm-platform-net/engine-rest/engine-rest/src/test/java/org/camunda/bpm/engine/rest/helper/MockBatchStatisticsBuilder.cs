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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using BatchStatistics = org.camunda.bpm.engine.batch.BatchStatistics;

	public class MockBatchStatisticsBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int size_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int jobsCreated_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int batchJobsPerSeed_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int invocationsPerBatchJob_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string seedJobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string monitorJobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchJobDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string createUserId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int remainingJobs_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int completedJobs_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int failedJobs_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Conflict;

	  public virtual MockBatchStatisticsBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder type(string type)
	  {
		this.type_Conflict = type;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder size(int size)
	  {
		this.size_Conflict = size;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder jobsCreated(int jobsCreated)
	  {
		this.jobsCreated_Conflict = jobsCreated;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Conflict = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Conflict = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Conflict = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Conflict = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Conflict = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder createUserId(string createUserId)
	  {
		this.createUserId_Conflict = createUserId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder remainingJobs(int remainingJobs)
	  {
		this.remainingJobs_Conflict = remainingJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder completedJobs(int completedJobs)
	  {
		this.completedJobs_Conflict = completedJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder failedJobs(int failedJobs)
	  {
		this.failedJobs_Conflict = failedJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder suspended()
	  {
		this.suspended_Conflict = true;
		return this;
	  }

	  public virtual BatchStatistics build()
	  {
		BatchStatistics batchStatistics = mock(typeof(BatchStatistics));
		when(batchStatistics.Id).thenReturn(id_Conflict);
		when(batchStatistics.Type).thenReturn(type_Conflict);
		when(batchStatistics.TotalJobs).thenReturn(size_Conflict);
		when(batchStatistics.JobsCreated).thenReturn(jobsCreated_Conflict);
		when(batchStatistics.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Conflict);
		when(batchStatistics.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Conflict);
		when(batchStatistics.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Conflict);
		when(batchStatistics.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Conflict);
		when(batchStatistics.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Conflict);
		when(batchStatistics.TenantId).thenReturn(tenantId_Conflict);
		when(batchStatistics.CreateUserId).thenReturn(createUserId_Conflict);
		when(batchStatistics.RemainingJobs).thenReturn(remainingJobs_Conflict);
		when(batchStatistics.CompletedJobs).thenReturn(completedJobs_Conflict);
		when(batchStatistics.FailedJobs).thenReturn(failedJobs_Conflict);
		when(batchStatistics.Suspended).thenReturn(suspended_Conflict);
		return batchStatistics;
	  }
	}

}