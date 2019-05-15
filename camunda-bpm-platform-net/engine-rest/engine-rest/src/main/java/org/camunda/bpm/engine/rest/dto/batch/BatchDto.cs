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
namespace org.camunda.bpm.engine.rest.dto.batch
{
	using Batch = org.camunda.bpm.engine.batch.Batch;

	public class BatchDto
	{

	  protected internal string id;
	  protected internal string type;
	  protected internal int totalJobs;
	  protected internal int jobsCreated;
	  protected internal int batchJobsPerSeed;
	  protected internal int invocationsPerBatchJob;
	  protected internal string seedJobDefinitionId;
	  protected internal string monitorJobDefinitionId;
	  protected internal string batchJobDefinitionId;
	  protected internal bool suspended;
	  protected internal string tenantId;
	  protected internal string createUserId;

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

	  public virtual int JobsCreated
	  {
		  get
		  {
			return jobsCreated;
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

	  public virtual bool Suspended
	  {
		  get
		  {
			return suspended;
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

	  public static BatchDto fromBatch(Batch batch)
	  {
		BatchDto dto = new BatchDto();
		dto.id = batch.Id;
		dto.type = batch.Type;
		dto.totalJobs = batch.TotalJobs;
		dto.jobsCreated = batch.JobsCreated;
		dto.batchJobsPerSeed = batch.BatchJobsPerSeed;
		dto.invocationsPerBatchJob = batch.InvocationsPerBatchJob;
		dto.seedJobDefinitionId = batch.SeedJobDefinitionId;
		dto.monitorJobDefinitionId = batch.MonitorJobDefinitionId;
		dto.batchJobDefinitionId = batch.BatchJobDefinitionId;
		dto.suspended = batch.Suspended;
		dto.tenantId = batch.TenantId;
		dto.createUserId = batch.CreateUserId;
		return dto;
	  }

	}

}