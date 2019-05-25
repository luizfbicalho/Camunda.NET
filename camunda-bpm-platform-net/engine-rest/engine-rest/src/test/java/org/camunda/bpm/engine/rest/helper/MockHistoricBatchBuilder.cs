using System;

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

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;

	public class MockHistoricBatchBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int totalJobs_Conflict;
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
	  protected internal DateTime startTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime endTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime removalTime_Conflict;

	  public virtual MockHistoricBatchBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder type(string type)
	  {
		this.type_Conflict = type;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder totalJobs(int totalJobs)
	  {
		this.totalJobs_Conflict = totalJobs;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Conflict = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Conflict = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Conflict = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Conflict = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Conflict = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder createUserId(string createUserId)
	  {
		this.createUserId_Conflict = createUserId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder startTime(DateTime startTime)
	  {
		this.startTime_Conflict = startTime;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder endTime(DateTime endTime)
	  {
		this.endTime_Conflict = endTime;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder removalTime(DateTime removalTime)
	  {
		this.removalTime_Conflict = removalTime;
		return this;
	  }

	  public virtual HistoricBatch build()
	  {
		HistoricBatch historicBatch = mock(typeof(HistoricBatch));
		when(historicBatch.Id).thenReturn(id_Conflict);
		when(historicBatch.Type).thenReturn(type_Conflict);
		when(historicBatch.TotalJobs).thenReturn(totalJobs_Conflict);
		when(historicBatch.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Conflict);
		when(historicBatch.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Conflict);
		when(historicBatch.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Conflict);
		when(historicBatch.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Conflict);
		when(historicBatch.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Conflict);
		when(historicBatch.TenantId).thenReturn(tenantId_Conflict);
		when(historicBatch.CreateUserId).thenReturn(createUserId_Conflict);
		when(historicBatch.StartTime).thenReturn(startTime_Conflict);
		when(historicBatch.EndTime).thenReturn(endTime_Conflict);
		when(historicBatch.RemovalTime).thenReturn(removalTime_Conflict);
		return historicBatch;
	  }

	}

}