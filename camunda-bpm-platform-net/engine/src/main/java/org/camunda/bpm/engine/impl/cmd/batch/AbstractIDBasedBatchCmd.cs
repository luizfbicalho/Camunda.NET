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
namespace org.camunda.bpm.engine.impl.cmd.batch
{
	using BatchConfiguration = org.camunda.bpm.engine.impl.batch.BatchConfiguration;
	using BatchEntity = org.camunda.bpm.engine.impl.batch.BatchEntity;
	using BatchJobHandler = org.camunda.bpm.engine.impl.batch.BatchJobHandler;
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using BatchUtil = org.camunda.bpm.engine.impl.util.BatchUtil;

	/// <summary>
	/// Representation of common logic to all Batch commands which are based on list of
	/// IDs.
	/// 
	/// @author Askar Akhmerov
	/// </summary>
	public abstract class AbstractIDBasedBatchCmd<T> : AbstractBatchCmd<T>
	{

	  protected internal virtual BatchEntity createBatch(CommandContext commandContext, IList<string> ids)
	  {
		ProcessEngineConfigurationImpl processEngineConfiguration = commandContext.ProcessEngineConfiguration;
		BatchJobHandler batchJobHandler = getBatchJobHandler(processEngineConfiguration);

		BatchConfiguration configuration = getAbstractIdsBatchConfiguration(ids);

		BatchEntity batch = new BatchEntity();
		batch.Type = batchJobHandler.Type;
		batch.TotalJobs = BatchUtil.calculateBatchSize(processEngineConfiguration, configuration);
		batch.BatchJobsPerSeed = processEngineConfiguration.BatchJobsPerSeed;
		batch.InvocationsPerBatchJob = processEngineConfiguration.InvocationsPerBatchJob;
		batch.ConfigurationBytes = batchJobHandler.writeConfiguration(configuration);
		commandContext.BatchManager.insertBatch(batch);

		return batch;
	  }

	  protected internal abstract BatchConfiguration getAbstractIdsBatchConfiguration(IList<string> ids);

	  protected internal abstract BatchJobHandler getBatchJobHandler(ProcessEngineConfigurationImpl processEngineConfiguration);
	}

}