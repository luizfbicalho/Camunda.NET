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
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string type_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal int totalJobs_Renamed;
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
	  protected internal DateTime startTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime endTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime removalTime_Renamed;

	  public virtual MockHistoricBatchBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder type(string type)
	  {
		this.type_Renamed = type;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder totalJobs(int totalJobs)
	  {
		this.totalJobs_Renamed = totalJobs;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder batchJobsPerSeed(int batchJobsPerSeed)
	  {
		this.batchJobsPerSeed_Renamed = batchJobsPerSeed;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder invocationsPerBatchJob(int invocationsPerBatchJob)
	  {
		this.invocationsPerBatchJob_Renamed = invocationsPerBatchJob;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder seedJobDefinitionId(string seedJobDefinitionId)
	  {
		this.seedJobDefinitionId_Renamed = seedJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder monitorJobDefinitionId(string monitorJobDefinitionId)
	  {
		this.monitorJobDefinitionId_Renamed = monitorJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder batchJobDefinitionId(string batchJobDefinitionId)
	  {
		this.batchJobDefinitionId_Renamed = batchJobDefinitionId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder createUserId(string createUserId)
	  {
		this.createUserId_Renamed = createUserId;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder startTime(DateTime startTime)
	  {
		this.startTime_Renamed = startTime;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder endTime(DateTime endTime)
	  {
		this.endTime_Renamed = endTime;
		return this;
	  }

	  public virtual MockHistoricBatchBuilder removalTime(DateTime removalTime)
	  {
		this.removalTime_Renamed = removalTime;
		return this;
	  }

	  public virtual HistoricBatch build()
	  {
		HistoricBatch historicBatch = mock(typeof(HistoricBatch));
		when(historicBatch.Id).thenReturn(id_Renamed);
		when(historicBatch.Type).thenReturn(type_Renamed);
		when(historicBatch.TotalJobs).thenReturn(totalJobs_Renamed);
		when(historicBatch.BatchJobsPerSeed).thenReturn(batchJobsPerSeed_Renamed);
		when(historicBatch.InvocationsPerBatchJob).thenReturn(invocationsPerBatchJob_Renamed);
		when(historicBatch.SeedJobDefinitionId).thenReturn(seedJobDefinitionId_Renamed);
		when(historicBatch.MonitorJobDefinitionId).thenReturn(monitorJobDefinitionId_Renamed);
		when(historicBatch.BatchJobDefinitionId).thenReturn(batchJobDefinitionId_Renamed);
		when(historicBatch.TenantId).thenReturn(tenantId_Renamed);
		when(historicBatch.CreateUserId).thenReturn(createUserId_Renamed);
		when(historicBatch.StartTime).thenReturn(startTime_Renamed);
		when(historicBatch.EndTime).thenReturn(endTime_Renamed);
		when(historicBatch.RemovalTime).thenReturn(removalTime_Renamed);
		return historicBatch;
	  }

	}

}