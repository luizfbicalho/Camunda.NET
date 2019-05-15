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
namespace org.camunda.bpm.engine.impl.batch
{

	using Batch = org.camunda.bpm.engine.batch.Batch;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntity = org.camunda.bpm.engine.impl.db.DbEntity;
	using HasDbReferences = org.camunda.bpm.engine.impl.db.HasDbReferences;
	using HasDbRevision = org.camunda.bpm.engine.impl.db.HasDbRevision;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using HistoricIncidentManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricIncidentManager;
	using HistoricJobLogManager = org.camunda.bpm.engine.impl.persistence.entity.HistoricJobLogManager;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using JobDefinitionManager = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using Nameable = org.camunda.bpm.engine.impl.persistence.entity.Nameable;
	using SuspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState;
	using ByteArrayField = org.camunda.bpm.engine.impl.persistence.entity.util.ByteArrayField;
	using ClockUtil = org.camunda.bpm.engine.impl.util.ClockUtil;
	using ResourceTypes = org.camunda.bpm.engine.repository.ResourceTypes;

	public class BatchEntity : Batch, DbEntity, HasDbReferences, Nameable, HasDbRevision
	{
		private bool InstanceFieldsInitialized = false;

		public BatchEntity()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			configuration = new ByteArrayField(this, ResourceTypes.RUNTIME);
		}


	  public static readonly BatchSeedJobDeclaration BATCH_SEED_JOB_DECLARATION = new BatchSeedJobDeclaration();
	  public static readonly BatchMonitorJobDeclaration BATCH_MONITOR_JOB_DECLARATION = new BatchMonitorJobDeclaration();

	  // persistent
	  protected internal string id;
	  protected internal string type;

	  protected internal int totalJobs;
	  protected internal int jobsCreated;
	  protected internal int batchJobsPerSeed;
	  protected internal int invocationsPerBatchJob;

	  protected internal string seedJobDefinitionId;
	  protected internal string monitorJobDefinitionId;
	  protected internal string batchJobDefinitionId;

	  protected internal ByteArrayField configuration;

	  protected internal string tenantId;
	  protected internal string createUserId;

	  protected internal int suspensionState = org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.ACTIVE.StateCode;

	  protected internal int revision;

	  // transient
	  protected internal JobDefinitionEntity seedJobDefinition;
	  protected internal JobDefinitionEntity monitorJobDefinition;
	  protected internal JobDefinitionEntity batchJobDefinition;

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected BatchJobHandler<?> batchJobHandler;
	  protected internal BatchJobHandler<object> batchJobHandler;

	  public virtual string Id
	  {
		  get
		  {
			return id;
		  }
		  set
		  {
			this.id = value;
		  }
	  }


	  public virtual string Name
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


	  public virtual int JobsCreated
	  {
		  get
		  {
			return jobsCreated;
		  }
		  set
		  {
			this.jobsCreated = value;
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


	  public virtual string Configuration
	  {
		  get
		  {
			return configuration.ByteArrayId;
		  }
		  set
		  {
			this.configuration.ByteArrayId = value;
		  }
	  }


	  public virtual int SuspensionState
	  {
		  set
		  {
			this.suspensionState = value;
		  }
		  get
		  {
			return suspensionState;
		  }
	  }


	  public virtual bool Suspended
	  {
		  get
		  {
			return suspensionState == org.camunda.bpm.engine.impl.persistence.entity.SuspensionState_Fields.SUSPENDED.StateCode;
		  }
	  }

	  public virtual int Revision
	  {
		  set
		  {
			this.revision = value;
    
		  }
		  get
		  {
			return revision;
		  }
	  }


	  public virtual int RevisionNext
	  {
		  get
		  {
			return revision + 1;
		  }
	  }

	  // transient

	  public virtual JobDefinitionEntity SeedJobDefinition
	  {
		  get
		  {
			if (seedJobDefinition == null && !string.ReferenceEquals(seedJobDefinitionId, null))
			{
			  seedJobDefinition = Context.CommandContext.JobDefinitionManager.findById(seedJobDefinitionId);
			}
    
			return seedJobDefinition;
		  }
	  }

	  public virtual JobDefinitionEntity MonitorJobDefinition
	  {
		  get
		  {
			if (monitorJobDefinition == null && !string.ReferenceEquals(monitorJobDefinitionId, null))
			{
			  monitorJobDefinition = Context.CommandContext.JobDefinitionManager.findById(monitorJobDefinitionId);
			}
    
			return monitorJobDefinition;
		  }
	  }

	  public virtual JobDefinitionEntity BatchJobDefinition
	  {
		  get
		  {
			if (batchJobDefinition == null && !string.ReferenceEquals(batchJobDefinitionId, null))
			{
			  batchJobDefinition = Context.CommandContext.JobDefinitionManager.findById(batchJobDefinitionId);
			}
    
			return batchJobDefinition;
		  }
	  }

	  public virtual sbyte[] ConfigurationBytes
	  {
		  get
		  {
			return this.configuration.getByteArrayValue();
		  }
		  set
		  {
			this.configuration.setByteArrayValue(value);
		  }
	  }


//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: public BatchJobHandler<?> getBatchJobHandler()
	  public virtual BatchJobHandler<object> BatchJobHandler
	  {
		  get
		  {
			if (batchJobHandler == null)
			{
			  batchJobHandler = Context.CommandContext.ProcessEngineConfiguration.BatchHandlers[type];
			}
    
			return batchJobHandler;
		  }
	  }

	  public virtual object PersistentState
	  {
		  get
		  {
			Dictionary<string, object> persistentState = new Dictionary<string, object>();
			persistentState["jobsCreated"] = jobsCreated;
			return persistentState;
		  }
	  }

	  public virtual JobDefinitionEntity createSeedJobDefinition()
	  {
		seedJobDefinition = new JobDefinitionEntity(BATCH_SEED_JOB_DECLARATION);
		seedJobDefinition.JobConfiguration = id;
		seedJobDefinition.TenantId = tenantId;

		Context.CommandContext.JobDefinitionManager.insert(seedJobDefinition);

		seedJobDefinitionId = seedJobDefinition.Id;

		return seedJobDefinition;
	  }

	  public virtual JobDefinitionEntity createMonitorJobDefinition()
	  {
		monitorJobDefinition = new JobDefinitionEntity(BATCH_MONITOR_JOB_DECLARATION);
		monitorJobDefinition.JobConfiguration = id;
		monitorJobDefinition.TenantId = tenantId;

		Context.CommandContext.JobDefinitionManager.insert(monitorJobDefinition);

		monitorJobDefinitionId = monitorJobDefinition.Id;

		return monitorJobDefinition;
	  }

	  public virtual JobDefinitionEntity createBatchJobDefinition()
	  {
		batchJobDefinition = new JobDefinitionEntity(BatchJobHandler.JobDeclaration);
		batchJobDefinition.JobConfiguration = id;
		batchJobDefinition.TenantId = tenantId;

		Context.CommandContext.JobDefinitionManager.insert(batchJobDefinition);

		batchJobDefinitionId = batchJobDefinition.Id;

		return batchJobDefinition;
	  }

	  public virtual JobEntity createSeedJob()
	  {
		JobEntity seedJob = BATCH_SEED_JOB_DECLARATION.createJobInstance(this);

		Context.CommandContext.JobManager.insertAndHintJobExecutor(seedJob);

		return seedJob;
	  }

	  public virtual void deleteSeedJob()
	  {
		IList<JobEntity> seedJobs = Context.CommandContext.JobManager.findJobsByJobDefinitionId(seedJobDefinitionId);

		foreach (JobEntity job in seedJobs)
		{
		  job.delete();
		}
	  }

	  public virtual JobEntity createMonitorJob(bool setDueDate)
	  {
		// Maybe use an other job declaration
		JobEntity monitorJob = BATCH_MONITOR_JOB_DECLARATION.createJobInstance(this);
		if (setDueDate)
		{
		  monitorJob.Duedate = calculateMonitorJobDueDate();
		}

		Context.CommandContext.JobManager.insertAndHintJobExecutor(monitorJob);

		return monitorJob;
	  }

	  protected internal virtual DateTime calculateMonitorJobDueDate()
	  {
		int pollTime = Context.CommandContext.ProcessEngineConfiguration.BatchPollTime;
		long dueTime = ClockUtil.CurrentTime.Ticks + (pollTime * 1000);
		return new DateTime(dueTime);
	  }

	  public virtual void deleteMonitorJob()
	  {
		IList<JobEntity> monitorJobs = Context.CommandContext.JobManager.findJobsByJobDefinitionId(monitorJobDefinitionId);

		foreach (JobEntity monitorJob in monitorJobs)
		{
		  monitorJob.delete();
		}
	  }

	  public virtual void delete(bool cascadeToHistory)
	  {
		CommandContext commandContext = Context.CommandContext;

		deleteSeedJob();
		deleteMonitorJob();
		BatchJobHandler.deleteJobs(this);

		JobDefinitionManager jobDefinitionManager = commandContext.JobDefinitionManager;
		jobDefinitionManager.delete(SeedJobDefinition);
		jobDefinitionManager.delete(MonitorJobDefinition);
		jobDefinitionManager.delete(BatchJobDefinition);

		commandContext.BatchManager.delete(this);
		configuration.deleteByteArrayValue();

		fireHistoricEndEvent();

		if (cascadeToHistory)
		{
		  HistoricIncidentManager historicIncidentManager = commandContext.HistoricIncidentManager;
		  historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(seedJobDefinitionId);
		  historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(monitorJobDefinitionId);
		  historicIncidentManager.deleteHistoricIncidentsByJobDefinitionId(batchJobDefinitionId);

		  HistoricJobLogManager historicJobLogManager = commandContext.HistoricJobLogManager;
		  historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(seedJobDefinitionId);
		  historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(monitorJobDefinitionId);
		  historicJobLogManager.deleteHistoricJobLogsByJobDefinitionId(batchJobDefinitionId);

		  commandContext.HistoricBatchManager.deleteHistoricBatchById(id);
		}
	  }

	  public virtual void fireHistoricStartEvent()
	  {
		Context.CommandContext.HistoricBatchManager.createHistoricBatch(this);
	  }

	  public virtual void fireHistoricEndEvent()
	  {
		Context.CommandContext.HistoricBatchManager.completeHistoricBatch(this);
	  }

	  public virtual bool Completed
	  {
		  get
		  {
			return Context.CommandContext.ProcessEngineConfiguration.ManagementService.createJobQuery().jobDefinitionId(batchJobDefinitionId).count() == 0;
		  }
	  }

	  public override string ToString()
	  {
		return "BatchEntity{" +
		  "batchHandler=" + batchJobHandler +
		  ", id='" + id + '\'' +
		  ", type='" + type + '\'' +
		  ", size=" + totalJobs +
		  ", jobCreated=" + jobsCreated +
		  ", batchJobsPerSeed=" + batchJobsPerSeed +
		  ", invocationsPerBatchJob=" + invocationsPerBatchJob +
		  ", seedJobDefinitionId='" + seedJobDefinitionId + '\'' +
		  ", monitorJobDefinitionId='" + seedJobDefinitionId + '\'' +
		  ", batchJobDefinitionId='" + batchJobDefinitionId + '\'' +
		  ", configurationId='" + configuration.ByteArrayId + '\'' +
		  '}';
	  }

	  public virtual ISet<string> ReferencedEntityIds
	  {
		  get
		  {
			ISet<string> referencedEntityIds = new HashSet<string>();
			return referencedEntityIds;
		  }
	  }

	  public virtual IDictionary<string, Type> ReferencedEntitiesIdAndClass
	  {
		  get
		  {
			IDictionary<string, Type> referenceIdAndClass = new Dictionary<string, Type>();
    
			if (!string.ReferenceEquals(seedJobDefinitionId, null))
			{
			  referenceIdAndClass[seedJobDefinitionId] = typeof(JobDefinitionEntity);
			}
			if (!string.ReferenceEquals(batchJobDefinitionId, null))
			{
			  referenceIdAndClass[batchJobDefinitionId] = typeof(JobDefinitionEntity);
			}
			if (!string.ReferenceEquals(monitorJobDefinitionId, null))
			{
			  referenceIdAndClass[monitorJobDefinitionId] = typeof(JobDefinitionEntity);
			}
    
			return referenceIdAndClass;
		  }
	  }
	}

}