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
namespace org.camunda.bpm.engine.impl.batch.history
{

	using HistoricBatch = org.camunda.bpm.engine.batch.history.HistoricBatch;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HistoryEvent = org.camunda.bpm.engine.impl.history.@event.HistoryEvent;
	using HistoricIncidentManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentManager;
	using HistoricJobLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogManager;

	[Serializable]
	public class HistoricBatchEntity : HistoryEvent, HistoricBatch, DbEntity
	{

	  private const long serialVersionUID = 1L;

	  protected internal new string id;
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

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
		  set
		  {
			this.type = value;
		  }
	  }


	  public virtual int TotalJobs
	  {
		  get
		  {
			return totalJobs;
		  }
		  set
		  {
			this.totalJobs = value;
		  }
	  }


	  public virtual int BatchJobsPerSeed
	  {
		  get
		  {
			return batchJobsPerSeed;
		  }
		  set
		  {
			this.batchJobsPerSeed = value;
		  }
	  }


	  public virtual int InvocationsPerBatchJob
	  {
		  get
		  {
			return invocationsPerBatchJob;
		  }
		  set
		  {
			this.invocationsPerBatchJob = value;
		  }
	  }


	  public virtual string SeedJobDefinitionId
	  {
		  get
		  {
			return seedJobDefinitionId;
		  }
		  set
		  {
			this.seedJobDefinitionId = value;
		  }
	  }


	  public virtual string MonitorJobDefinitionId
	  {
		  get
		  {
			return monitorJobDefinitionId;
		  }
		  set
		  {
			this.monitorJobDefinitionId = value;
		  }
	  }


	  public virtual string BatchJobDefinitionId
	  {
		  get
		  {
			return batchJobDefinitionId;
		  }
		  set
		  {
			this.batchJobDefinitionId = value;
		  }
	  }


	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId;
		  }
		  set
		  {
			this.tenantId = value;
		  }
	  }


	  public virtual string CreateUserId
	  {
		  get
		  {
			return createUserId;
		  }
		  set
		  {
			this.createUserId = value;
		  }
	  }


	  public virtual DateTime StartTime
	  {
		  get
		  {
			return startTime;
		  }
		  set
		  {
			this.startTime = value;
		  }
	  }


	  public virtual DateTime EndTime
	  {
		  get
		  {
			return endTime;
		  }
		  set
		  {
			this.endTime = value;
		  }
	  }


	  public override object PersistentState
	  {
		  get
		  {
			IDictionary<string, object> persistentState = new Dictionary<string, object>();
    
			persistentState["endTime"] = endTime;
    
			return persistentState;
		  }
	  }

	  public virtual void delete()
	  {
		HistoricIncidentManager historicIncidentManager = Context.CommandContext.HistoricIncidentManager;
		historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(seedJobDefinitionId);
		historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(monitorJobDefinitionId);
		historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(batchJobDefinitionId);

		HistoricJobLogManager historicJobLogManager = Context.CommandContext.HistoricJobLogManager;
		historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(seedJobDefinitionId);
		historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(monitorJobDefinitionId);
		historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(batchJobDefinitionId);

		Context.CommandContext.HistoricBatchManager.delete(this);
	  }

	}

}