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
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using JobDeclaration = org.camunda.bpm.engine.impl.jobexecutor.JobDeclaration;
	using JsonObjectConverter = org.camunda.bpm.engine.impl.json.JsonObjectConverter;
	using ByteArrayEntity = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayEntity;
	using ByteArrayManager = org.camunda.bpm.engine.impl.persistence.entity.ByteArrayManager;
	using JobEntity = org.camunda.bpm.engine.impl.persistence.entity.JobEntity;
	using JobManager = org.camunda.bpm.engine.impl.persistence.entity.JobManager;
	using MessageEntity = org.camunda.bpm.engine.impl.persistence.entity.MessageEntity;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonElement = com.google.gson.JsonElement;

	/// <summary>
	/// Common methods for batch job handlers based on list of ids, providing serialization, configuration instantiation, etc.
	/// 
	/// @author Askar Akhmerov
	/// </summary>
	public abstract class AbstractBatchJobHandler<T> : BatchJobHandler<T> where T : BatchConfiguration
	{

	  public abstract JobDeclaration<BatchJobContext, MessageEntity> JobDeclaration {get;}

	  public virtual bool createJobs(BatchEntity batch)
	  {
		CommandContext commandContext = Context.CommandContext;
		ByteArrayManager byteArrayManager = commandContext.ByteArrayManager;
		JobManager jobManager = commandContext.JobManager;

		T configuration = readConfiguration(batch.ConfigurationBytes);

		int batchJobsPerSeed = batch.BatchJobsPerSeed;
		int invocationsPerBatchJob = batch.InvocationsPerBatchJob;

		IList<string> ids = configuration.Ids;
		int numberOfItemsToProcess = Math.Min(invocationsPerBatchJob * batchJobsPerSeed, ids.Count);
		// view of process instances to process
		IList<string> processIds = ids.subList(0, numberOfItemsToProcess);

		int createdJobs = 0;
		while (processIds.Count > 0)
		{
		  int lastIdIndex = Math.Min(invocationsPerBatchJob, processIds.Count);
		  // view of process instances for this job
		  IList<string> idsForJob = processIds.subList(0, lastIdIndex);

		  T jobConfiguration = createJobConfiguration(configuration, idsForJob);
		  ByteArrayEntity configurationEntity = saveConfiguration(byteArrayManager, jobConfiguration);

		  JobEntity job = createBatchJob(batch, configurationEntity);
		  postProcessJob(configuration, job);
		  jobManager.insertAndHintJobExecutor(job);

		  idsForJob.Clear();
		  createdJobs++;
		}

		// update created jobs for batch
		batch.JobsCreated = batch.JobsCreated + createdJobs;

		// update batch configuration
		batch.ConfigurationBytes = writeConfiguration(configuration);

		return ids.Count == 0;
	  }

	  protected internal abstract T createJobConfiguration(T configuration, IList<string> processIdsForJob);

	  protected internal virtual void postProcessJob(T configuration, JobEntity job)
	  {
		// do nothing as default
	  }


	  protected internal virtual JobEntity createBatchJob(BatchEntity batch, ByteArrayEntity configuration)
	  {
		BatchJobContext creationContext = new BatchJobContext(batch, configuration);
		return JobDeclaration.createJobInstance(creationContext);
	  }

	  public virtual void deleteJobs(BatchEntity batch)
	  {
		IList<JobEntity> jobs = Context.CommandContext.JobManager.findJobsByJobDefinitionId(batch.BatchJobDefinitionId);

		foreach (JobEntity job in jobs)
		{
		  job.delete();
		}
	  }

	  public override BatchJobConfiguration newConfiguration(string canonicalString)
	  {
		return new BatchJobConfiguration(canonicalString);
	  }

	  public override void onDelete(BatchJobConfiguration configuration, JobEntity jobEntity)
	  {
		string byteArrayId = configuration.ConfigurationByteArrayId;
		if (!string.ReferenceEquals(byteArrayId, null))
		{
		  Context.CommandContext.ByteArrayManager.deleteByteArrayById(byteArrayId);
		}
	  }

	  protected internal virtual ByteArrayEntity saveConfiguration(ByteArrayManager byteArrayManager, T jobConfiguration)
	  {
		ByteArrayEntity configurationEntity = new ByteArrayEntity();
		configurationEntity.Bytes = writeConfiguration(jobConfiguration);
		byteArrayManager.insert(configurationEntity);
		return configurationEntity;
	  }

	  public virtual sbyte[] writeConfiguration(T configuration)
	  {
		JsonElement jsonObject = JsonConverterInstance.toJsonObject(configuration);

		return JsonUtil.asBytes(jsonObject);
	  }

	  public virtual T readConfiguration(sbyte[] serializedConfiguration)
	  {
		return JsonConverterInstance.toObject(JsonUtil.asObject(serializedConfiguration));
	  }

	  protected internal abstract JsonObjectConverter<T> JsonConverterInstance {get;}
	}

}