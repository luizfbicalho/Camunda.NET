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
namespace org.camunda.bpm.engine.rest.dto.history.batch
{

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;

	public class HistoricBatchDto
	{

	  protected internal string id;
	  protected internal string type;
	  protected internal int totalJobs;
	  protected internal int batchJobsPerSeed;
	  protected internal int invocationsPerBatchJob;
	  protected internal string seedJobDefinitionId;
	  protected internal string monitorJobDefinitionId;
	  protected internal string batchJobDefinitionId;
	  protected internal string tenantId;
	  protected internal string createUserId;
	  protected internal DateTime startTime;
	  protected internal DateTime endTime;
	  protected internal DateTime removalTime;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual int TotalJobs
	  {
		  get
		  {
			return totalJobs;
		  }
	  }

	  public virtual int BatchJobsPerSeed
	  {
		  get
		  {
			return batchJobsPerSeed;
		  }
	  }

	  public virtual int InvocationsPerBatchJob
	  {
		  get
		  {
			return invocationsPerBatchJob;
		  }
	  }

	  public virtual string SeedJobDefinitionId
	  {
		  get
		  {
			return seedJobDefinitionId;
		  }
	  }

	  public virtual string MonitorJobDefinitionId
	  {
		  get
		  {
			return monitorJobDefinitionId;
		  }
	  }

	  public virtual string BatchJobDefinitionId
	  {
		  get
		  {
			return batchJobDefinitionId;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
	  }

	  public virtual string CreateUserId
	  {
		  get
		  {
			return createUserId;
		  }
	  }

	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
	  }

	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime;
		  }
	  }

	  public static HistoricBatchDto fromBatch(HistoricBatch historicBatch)
	  {
		HistoricBatchDto dto = new HistoricBatchDto();
		dto.id = historicBatch.Id;
		dto.type = historicBatch.Type;
		dto.totalJobs = historicBatch.TotalJobs;
		dto.batchJobsPerSeed = historicBatch.BatchJobsPerSeed;
		dto.invocationsPerBatchJob = historicBatch.InvocationsPerBatchJob;
		dto.seedJobDefinitionId = historicBatch.SeedJobDefinitionId;
		dto.monitorJobDefinitionId = historicBatch.MonitorJobDefinitionId;
		dto.batchJobDefinitionId = historicBatch.BatchJobDefinitionId;
		dto.tenantId = historicBatch.TenantId;
		dto.createUserId = historicBatch.CreateUserId;
		dto.startTime = historicBatch.StartTime;
		dto.endTime = historicBatch.EndTime;
		dto.removalTime = historicBatch.RemovalTime;
		return dto;
	  }

	}

}