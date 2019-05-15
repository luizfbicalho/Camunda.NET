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
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int size_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int jobsCreated_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int batchJobsPerSeed_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int invocationsPerBatchJob_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string seedJobDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string monitorJobDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string batchJobDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string createUserId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int remainingJobs_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int completedJobs_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int failedJobs_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool suspended_Renamed;

	  public virtual MockBatchStatisticsBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder type(string type)
	  {
		this.type_Renamed = type;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder size(int size)
	  {
		this.size_Renamed = size;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder jobsCreated(int jobsCreated)
	  {
		this.jobsCreated_Renamed = jobsCreated;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Renamed = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Renamed = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Renamed = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Renamed = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Renamed = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder createUserId(string createUserId)
	  {
		this.createUserId_Renamed = createUserId;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder remainingJobs(int remainingJobs)
	  {
		this.remainingJobs_Renamed = remainingJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder completedJobs(int completedJobs)
	  {
		this.completedJobs_Renamed = completedJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder failedJobs(int failedJobs)
	  {
		this.failedJobs_Renamed = failedJobs;
		return this;
	  }

	  public virtual MockBatchStatisticsBuilder suspended()
	  {
		this.suspended_Renamed = true;
		return this;
	  }

	  public virtual BatchStatistics build()
	  {
		BatchStatistics batchStatistics = mock(typeof(BatchStatistics));
		when(batchStatistics.Id).thenReturn(id_Renamed);
		when(batchStatistics.Type).thenReturn(type_Renamed);
		when(batchStatistics.TotalJobs).thenReturn(size_Renamed);
		when(batchStatistics.JobsCreated).thenReturn(jobsCreated_Renamed);
		when(batchStatistics.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Renamed);
		when(batchStatistics.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Renamed);
		when(batchStatistics.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Renamed);
		when(batchStatistics.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Renamed);
		when(batchStatistics.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Renamed);
		when(batchStatistics.TenantId).thenReturn(tenantId_Renamed);
		when(batchStatistics.CreateUserId).thenReturn(createUserId_Renamed);
		when(batchStatistics.RemainingJobs).thenReturn(remainingJobs_Renamed);
		when(batchStatistics.CompletedJobs).thenReturn(completedJobs_Renamed);
		when(batchStatistics.FailedJobs).thenReturn(failedJobs_Renamed);
		when(batchStatistics.Suspended).thenReturn(suspended_Renamed);
		return batchStatistics;
	  }
	}

}