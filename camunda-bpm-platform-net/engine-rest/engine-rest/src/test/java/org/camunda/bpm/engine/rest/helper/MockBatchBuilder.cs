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

	using Batch = org.camunda.bpm.engine.batch.Batch;

	public class MockBatchBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int totalJobs_Renamed;
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
	  protected internal bool suspended_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string createUserId_Renamed;

	  public virtual MockBatchBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockBatchBuilder type(string type)
	  {
		this.type_Renamed = type;
		return this;
	  }

	  public virtual MockBatchBuilder totalJobs(int totalJobs)
	  {
		this.totalJobs_Renamed = totalJobs;
		return this;
	  }

	  public virtual MockBatchBuilder jobsCreated(int jobsCreated)
	  {
		this.jobsCreated_Renamed = jobsCreated;
		return this;
	  }

	  public virtual MockBatchBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Renamed = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockBatchBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Renamed = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockBatchBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Renamed = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Renamed = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Renamed = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder suspended()
	  {
		this.suspended_Renamed = true;
		return this;
	  }

	  public virtual MockBatchBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockBatchBuilder createUserId(string createUserId)
	  {
		this.createUserId_Renamed = createUserId;
		return this;
	  }

	  public virtual Batch build()
	  {
		Batch batch = mock(typeof(Batch));
		when(batch.Id).thenReturn(id_Renamed);
		when(batch.Type).thenReturn(type_Renamed);
		when(batch.TotalJobs).thenReturn(totalJobs_Renamed);
		when(batch.JobsCreated).thenReturn(jobsCreated_Renamed);
		when(batch.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Renamed);
		when(batch.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Renamed);
		when(batch.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Renamed);
		when(batch.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Renamed);
		when(batch.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Renamed);
		when(batch.Suspended).thenReturn(suspended_Renamed);
		when(batch.TenantId).thenReturn(tenantId_Renamed);
		when(batch.CreateUserId).thenReturn(createUserId_Renamed);
		return batch;
	  }

	}

}