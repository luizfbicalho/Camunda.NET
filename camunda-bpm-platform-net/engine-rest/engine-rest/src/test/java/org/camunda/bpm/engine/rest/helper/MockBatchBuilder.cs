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
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int totalJobs_Conflict;
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
	  protected internal bool suspended_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string createUserId_Conflict;

	  public virtual MockBatchBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockBatchBuilder type(string type)
	  {
		this.type_Conflict = type;
		return this;
	  }

	  public virtual MockBatchBuilder totalJobs(int totalJobs)
	  {
		this.totalJobs_Conflict = totalJobs;
		return this;
	  }

	  public virtual MockBatchBuilder jobsCreated(int jobsCreated)
	  {
		this.jobsCreated_Conflict = jobsCreated;
		return this;
	  }

	  public virtual MockBatchBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Conflict = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockBatchBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Conflict = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockBatchBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Conflict = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Conflict = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Conflict = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockBatchBuilder suspended()
	  {
		this.suspended_Conflict = true;
		return this;
	  }

	  public virtual MockBatchBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockBatchBuilder createUserId(string createUserId)
	  {
		this.createUserId_Conflict = createUserId;
		return this;
	  }

	  public virtual Batch build()
	  {
		Batch batch = mock(typeof(Batch));
		when(batch.Id).thenReturn(id_Conflict);
		when(batch.Type).thenReturn(type_Conflict);
		when(batch.TotalJobs).thenReturn(totalJobs_Conflict);
		when(batch.JobsCreated).thenReturn(jobsCreated_Conflict);
		when(batch.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Conflict);
		when(batch.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Conflict);
		when(batch.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Conflict);
		when(batch.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Conflict);
		when(batch.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Conflict);
		when(batch.Suspended).thenReturn(suspended_Conflict);
		when(batch.TenantId).thenReturn(tenantId_Conflict);
		when(batch.CreateUserId).thenReturn(createUserId_Conflict);
		return batch;
	  }

	}

}